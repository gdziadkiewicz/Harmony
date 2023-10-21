using Mono.Cecil;
using System;
using System.IO;
using System.Reflection;

namespace HarmonyLib
{
	/// <summary>
	/// Patched code from https://github.com/MonoMod/MonoMod.Common/blob/ea24867bb49621372eaf53b2ef552cbf488af55b/Utils/ReflectionHelper.cs#L53-L88
	/// </summary>
	public static class ReflectionHelperPatched
	{
		/// <summary>
		/// We were sometimes getting null from the original ms.GetBuffer() call (and ArgumentNullException Array is null deeper inside nLoadAssembly), which kind of make sense given that
		/// https://learn.microsoft.com/en-us/dotnet/api/system.io.memorystream.getbuffer?view=net-7.0#:~:text=The%20buffer%20can%20also%20be%20null.
		/// says that the buffer can be null, but does not specify when :shrug:
		/// Here we strongly hope that ToArray() will never return null and that the Assembly.Load(byte[]) will always work
		/// </summary>
		public static Assembly Load(ModuleDefinition module)
		{
			using var stream = new MemoryStream();
			module.Write(stream);
			var asm = Assembly.Load(stream.ToArray());
			AppDomain.CurrentDomain.AssemblyResolve +=
				(s, e) => e.Name == asm.FullName ? asm : null;

			return asm;
		}
	}
}
