using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace Chen.Commons;
public static class ReflectionHelper
{
    // 是否是微软官方的程序集
    private static bool IsSystemAssembly(Assembly asm)
    {
        // 获取指定的程序集自定义特性，这里获取的是 AssemblyCompanyAttribute 特性
        var asmCompanyAttr = asm.GetCustomAttribute<AssemblyCompanyAttribute>();

        if (asmCompanyAttr == null)
        {
            // 如果没有获取到程序集的 AssemblyCompanyAttribute 特性，则认为不是系统程序集，返回false
            return false;
        }
        else
        {
            // 获取公司名称属性
            string companyName = asmCompanyAttr.Company;
            // 判断公司名称是否包含 "Microsoft"，如果包含则认为是系统程序集，返回 true，否则返回 false
            return companyName.Contains("Microsoft");
        }
    }

    private static bool IsSystemAssembly(string assemblyPath)
    {
        // 从指定路径的文件中加载 .NET 模块定义。
        var moduleDef = AsmResolver.DotNet.ModuleDefinition.FromFile(assemblyPath);

        // 获取模块定义对应的程序集。
        var assembly = moduleDef.Assembly;

        if (assembly == null)
        {
            // 如果无法获取到程序集，则认为不是系统程序集，返回false
            return false;
        }

        // 查找程序集中是否包含 AssembyCompanyAttribute 自定义特性
        var assemblyCompanyAttr = assembly.CustomAttributes.FirstOrDefault(c => 
        c.Constructor?.DeclaringType?.FullName == typeof(AssemblyCompanyAttribute).FullName);

        if (assemblyCompanyAttr == null)
        {
            return false; // 如果程序集中不包含 AssemblyCompanyAttribute 自定义特性，则认为不是系统程序集，返回false
        }

        // 获取 AssemblyCompanyAttribute 自定义特性中的公司名称属性
        var companyName = ((AsmResolver.Utf8String?)assemblyCompanyAttr.Signature?.FixedArguments[0]?.Element)?.Value;

        if (companyName == null)
        {
            return false; // 如果无法获取到公司名称属性，则认为不是系统程序集，返回 false。
        }

        // 判断公司名称是否包含 "Microsoft"，如果包含则认为是系统程序集，返回 true，否则返回 false
        return companyName.Contains("Microsoft");
    }

    /// <summary>
    /// 判断该文件是否是程序集
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    private static bool IsManagedAssmebly(string filePath)
    {
        // 使用 using 语句打开文件并创建文件流 fs，确保文件流在使用后能够被正确关闭
        using var fs = File.OpenRead(filePath);

        // 是用 using 语句创建 PEReader 对象 peReader，并确保在使用后能够被正确释放。
        using PEReader peReader = new PEReader(fs);

        // 返回一个布尔值， 指示打开的文件是否包含元数据，并且其中的元数据是否表示一个程序集
        return peReader.HasMetadata && peReader.GetMetadataReader().IsAssembly;
    }

    /// <summary>
    /// 尝试加载程序集
    /// </summary>
    /// <param name="assmeblyPath"></param>
    /// <returns></returns>
    private static Assembly? TryLoadAssembly(string assmeblyPath)
    {
        AssemblyName assemblyName = AssemblyName.GetAssemblyName(assmeblyPath);
        Assembly? assembly = null;
        try
        {
            assembly = Assembly.Load(assemblyName);
        }
        catch (BadImageFormatException ex)
        {
            Debug.WriteLine(ex);
        }
        catch (FileLoadException ex)
        {
            Debug.WriteLine(ex);
        }

        if (assembly == null)
        {
            try
            {
                assembly = Assembly.LoadFile(assmeblyPath);
            }
            catch (BadImageFormatException ex)
            {
                Debug.WriteLine(ex);
            }
            catch (FileLoadException ex)
            {
                Debug.WriteLine(ex);
            }
        }

        return assembly;

    }

    public static IEnumerable<Assembly> GetAllReferencedAssemblies(bool skipSystemAssemblies = true)
    {
        // 获取应用程序的根程序集
        Assembly? rootAssembly = Assembly.GetEntryAssembly();
        if (rootAssembly == null)
        {
            rootAssembly = Assembly.GetCallingAssembly();
        }

        // 创建一个集合来储存返回的程序集，并定义程序集相等比较器
        var returnAssemblies = new HashSet<Assembly>(new AssemblyEquality());

        // 创建集合用于跟踪已加载的程序集和已加载的程序集文件名
        var loadedAssemlies = new HashSet<string>();

        // 创建队列来储存待检查的程序集
        var assembliesToCheck = new Queue<Assembly>();
        assembliesToCheck.Enqueue(rootAssembly);

        // 如果需要跳过系统程序集，并且当前程序集是系统程序集，将其加入返回集合
        if (skipSystemAssemblies && IsSystemAssembly(rootAssembly))
        {
            if (IsValid(rootAssembly))
            {
                returnAssemblies.Add(rootAssembly);
            }
        }

        // 遍历待查询的程序集队列
        while (assembliesToCheck.Any())
        {
            // 从队列中取出一个待检查的程序集进行检查。
            var assemblyToCheck = assembliesToCheck.Dequeue();

            // 遍历待查询程序集所引用的所有程序集。
            foreach (var reference in assemblyToCheck.GetReferencedAssemblies())
            {
                // 如果已加载的程序集中不包含当前引用程序集的全名，则需要加载这个引用程序集。
                if (!loadedAssemlies.Contains(reference.FullName))
                {
                    // 加载引用的程序集
                    var assmbly = Assembly.Load(reference);

                    // 如果需要跳过系统程序集且当前加载的程序集是系统程序集，则直接跳过，不做处理。
                    if (skipSystemAssemblies && IsSystemAssembly(assmbly))
                    {
                        continue;
                    }

                    // 将加载的引用程序集加入待检查的程序集队列。
                    assembliesToCheck.Enqueue(assmbly);

                    // 将加载的程序集标记为已加载
                    loadedAssemlies.Add(reference.FullName);

                    // 如果加载的引用程序集有效，则将其添加到有效程序集中。
                    if (IsValid(assmbly))
                    {
                        returnAssemblies.Add(assmbly);
                    }
                }
            }
        }

        // 获取基本目录中的所有 dll 文件。
        var assmblyInBaseDir = Directory.EnumerateFiles(AppContext.BaseDirectory, "*.dll",
            new EnumerationOptions { RecurseSubdirectories = true });

        // 遍历目录中的dll文件。
        foreach (var assmblyPath in assmblyInBaseDir)
        {
            if (!IsManagedAssmebly(assmblyPath))
            {
                continue;
            }

            AssemblyName assemblyName = AssemblyName.GetAssemblyName(assmblyPath);

            // 如果程序集已经加载过了就不再加载
            if (returnAssemblies.Any(x => AssemblyName.ReferenceMatchesDefinition(x.GetName(),assemblyName)))
            {
                continue;
            }

            if (skipSystemAssemblies && IsSystemAssembly(assmblyPath))
            {
                continue;
            }

            // 尝试加载程序集
            Assembly? assembly = TryLoadAssembly(assmblyPath);
            if (assembly == null)
            {
                continue;
            }
            if (!IsValid(assembly))
            {
                continue;
            }
            if (skipSystemAssemblies && IsSystemAssembly(assembly))
            {
                continue;
            }
            returnAssemblies.Add(assembly);
        }
        return returnAssemblies.ToArray();
    }

    /// <summary>
    /// 程序集是否合法
    /// </summary>
    /// <param name="asm"></param>
    /// <returns></returns>
    private static bool IsValid(Assembly asm)
    {
        try
        {
            // 尝试获取程序集中定义的所有类型，如果程序集能够成功加载所有类型，则不会抛出异常。
            asm.GetTypes();

            // 获取程序集中定义的所有类型并转换为列表，如果程序集能够成功加载所有类型，则不会抛出异常。
            asm.DefinedTypes.ToList();

            // 如果程序集能够成功加载所有类型，则认为是有效的程序集，返回true。
            return true;
        }
        catch (ReflectionTypeLoadException)
        {
            return false;
        }
    }

    class AssemblyEquality : EqualityComparer<Assembly>
    {
        // 重写 Equals 方法来判断两个 Assmbly 是否相等
        public override bool Equals(Assembly? x, Assembly? y)
        {
            if (x == null && y == null) return true; 
            if (x == null || y == null) return false;
            return AssemblyName.ReferenceMatchesDefinition(x.GetName(),y.GetName());
            // 使用 AssemblyName.ReferenceMatchesDefinition() 方法来判断两个 Assmbly 的名称是否相等
        }
        // 重写 GetHashCode 方法来获取 Assembly 的哈希码
        public override int GetHashCode([DisallowNull] Assembly obj)
        {
            return obj.GetName().FullName.GetHashCode(); // 使用 Assembly 的全名（FullName）作为哈希码
        }
    }

}
