using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using RoslynMacros.Common.Interfaces;

namespace RoslynMacros.Common.Walkers
{
    public abstract class AbsWalker<WR> : IWalker<WR> where WR : IWalkerResult
    {
        public IDataEngine Engine { get; }

        protected AbsWalker(IDataEngine engine)
        {
            Engine = engine;
        }

        public IList<WR> Results { get; } = new List<WR>();
        public abstract void Visit(SyntaxTree st);
    }
}