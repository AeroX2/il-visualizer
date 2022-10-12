using System;
using System.Collections.Generic;
using System.Reflection;

namespace ILDebugging.Decoder
{
    [Serializable]
    public class MethodBodyInfo
    {
        public int Identity { get; set; }
        public string TypeName { get; set; }
        public string MethodToString { get; set; }
        public List<string> Instructions { get; } = new List<string>();

        private void AddInstruction(string inst)
        {
            Instructions.Add(inst);
        }

        public static MethodBodyInfo Create(MethodBase method)
        {
            var mbi = new MethodBodyInfo
            {
                Identity = method.GetHashCode(),
                TypeName = method.GetType().Name,
                MethodToString = method.ToString()
            };

            var visitor = new ReadableILStringVisitor(
                new MethodBodyInfoBuilder(mbi),
                DefaultFormatProvider.Instance);

            var reader = ILReaderFactory.Create(method);
            reader.Accept(visitor);

            return mbi;
        }

        private class MethodBodyInfoBuilder : IILStringCollector
        {
            private readonly MethodBodyInfo m_mbi;

            public MethodBodyInfoBuilder(MethodBodyInfo mbi)
            {
                m_mbi = mbi;
            }

            public void Process(ILInstruction instruction, string operandString)
            {
                m_mbi.AddInstruction($"IL_{instruction.Offset:x4}: {instruction.OpCode.Name,-10} {operandString}");
            }
        }
    }
}