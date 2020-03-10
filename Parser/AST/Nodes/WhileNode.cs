﻿using Parser.ASTVisitor;
using Parser.SymbolTable.Function;

namespace Parser.AST.Nodes
{
    public class WhileNode : ASTNodeBase
    {
        public FunctionSymbolTable Table { get; set; }

        public override void Accept(IVisitor v)
        {
            v.Visit(this);
        }

        protected override ASTNodeBase CreateNode()
        {
            return new WhileNode();
        }
    }
}
