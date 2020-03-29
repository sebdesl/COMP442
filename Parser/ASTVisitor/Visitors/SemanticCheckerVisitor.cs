﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Parser.AST;
using Parser.AST.Nodes;
using Parser.SymbolTable;
using Parser.SymbolTable.Class;
using Parser.SymbolTable.Function;
using Parser.Utils;

namespace Parser.ASTVisitor.Visitors
{
    public class SemanticCheckerVisitor : IVisitor
    {
        // Find a batter place for this
        public const string BoolType = "bool";

        private StreamWriter _errorStream;
        private GlobalSymbolTable _globalTable;

        public SemanticCheckerVisitor(StreamWriter errorStream, GlobalSymbolTable globalTable)
        {
            _globalTable = globalTable;
            _errorStream = errorStream;
        }

        public void Visit(ProgramNode n)
        {
            var children = n.GetChildren();
            foreach (var child in children)
            {
                child.Accept(this);
            }
        }

        public void Visit(ClassDeclsNode n)
        {
            var children = n.GetChildren();
            foreach (var child in children)
            {
                child.Accept(this);
            }
        }

        public void Visit(ClassDeclNode n)
        {
            foreach (var node in n.GetChildren())
            {
                node.SymTable = n.Table;
                node.Accept(this);
            }
        }

        public void Visit(FuncDefsNode n)
        {
            var children = n.GetChildren();
            foreach (var child in children)
            {
                child.Accept(this);
            }
        }

        public void Visit(IdentifierNode n)
        {
            foreach (var node in n.GetChildren())
            {
                node.SymTable = n.SymTable;
                node.Accept(this);
            }
        }

        public void Visit(FuncDefNode n)
        {
            var funcBody = (FuncBodyNode)n.GetChildren().Last();
            funcBody.SymTable = n.Table;
            funcBody.Accept(this);
        }

        public void Visit(FuncBodyNode n)
        {
            var statements = (StatementsNode)n.GetChildren().Last();
            statements.SymTable = n.SymTable;
            statements.Accept(this);
        }

        #region Statements
        public void Visit(StatementsNode n)
        {
            // TODO

            foreach (var node in n.GetChildren())
            {
                node.SymTable = n.SymTable;
                node.Accept(this);
            }
        }

        public void Visit(IfNode n)
        {
            //TODO 

            foreach (var node in n.GetChildren())
            {
                node.SymTable = n.SymTable;
                node.Accept(this);
            }
        }

        public void Visit(WhileNode n)
        {
            foreach (var node in n.GetChildren())
            {
                node.SymTable = n.SymTable;
                node.Accept(this);
            }
        }

        public void Visit(ReadNode n)
        {
            foreach (var node in n.GetChildren())
            {
                node.SymTable = n.SymTable;
                node.Accept(this);
            }
        }

        public void Visit(WriteNode n)
        {
            foreach (var node in n.GetChildren())
            {
                node.SymTable = n.SymTable;
                node.Accept(this);
            }
        }

        public void Visit(ReturnNode n)
        {
            var children = n.GetChildren();
            foreach (var node in children)
            {
                node.SymTable = n.SymTable;
                node.Accept(this);
            }

            var table = (FunctionSymbolTableEntry)n.SymTable;
            if (!string.Equals(children.Last().ExprType.Type, table.ReturnType.Lexeme))
            {
                _errorStream.WriteLine($"Type of value for return is invalid (line: {n.Token.StartLine}), expected: {table.ReturnType.Lexeme}, received: {children.Last().ExprType.Type}");
            }
        }
        #endregion


        public void Visit(BoolExpressionNode n)
        {
            foreach (var node in n.GetChildren())
            {
                node.SymTable = n.SymTable;
                node.Accept(this);
            }

            // TODO(AFL): Type check
        }


        private KeyValuePair<string, int> ExtractSubVarCallNode(SubVarCallNode node)
        {
            var children = node.GetChildren();
            var name = ((IdentifierNode)children[0]).Token.Lexeme;
            var numDims = ((IndicesNode)children[1]).GetChildren().Count;

            return new KeyValuePair<string, int>(name, numDims);
        }

        

        public void Visit(NotNode n)
        {
            var child = n.GetChildren().Single();
            child.Accept(this);
            n.ExprType = child.ExprType;
        }

        public void Visit(SignNode n)
        {
            var child = n.GetChildren().Single();
            child.Accept(this);
            n.ExprType = child.ExprType;
        }



        public void Visit(AddOpNode n)
        {
            var children = n.GetChildren();
            foreach (var node in children)
            {
                node.SymTable = n.SymTable;
                node.Accept(this);
            }

            if (children[0].ExprType.Dims.Count > 0 || children[1].ExprType.Dims.Count > 0)
            {
                _errorStream.WriteLine($"Cannot multiply/divide arrays! ({n.Token.StartLine}:{n.Token.StartColumn})");
            }
            else if (!string.Equals(children[0].ExprType.Type, children[1].ExprType.Type))
            {
                _errorStream.WriteLine($"Operand types don't match! {children[1].ExprType.Type} <-> {children[0].ExprType.Type} ({n.Token.StartLine}:{n.Token.StartColumn})");
            }

            n.ExprType = children[0].ExprType;
            
        }

        public void Visit(MultOpNode n)
        {

            var children = n.GetChildren();
            foreach (var node in n.GetChildren())
            {
                node.SymTable = n.SymTable;
                node.Accept(this);
            }

            if (children[0].ExprType.Dims.Count > 0 || children[1].ExprType.Dims.Count > 0)
            {
                _errorStream.WriteLine($"Cannot multiply/divide arrays! ({n.Token.StartLine}:{n.Token.StartColumn})");
            }
            else if (!string.Equals(children[0].ExprType.Type, children[1].ExprType.Type))
            {
                _errorStream.WriteLine($"Operand types don't match! {children[1].ExprType.Type} <-> {children[0].ExprType.Type} ({n.Token.StartLine}:{n.Token.StartColumn})");
            }

            n.ExprType = children[0].ExprType;
        }







        public void Visit(NullNode n)
        {
            foreach (var node in n.GetChildren())
            {
                node.SymTable = n.SymTable;
                node.Accept(this);
            }
        }

        public void Visit(MemberDeclsNode n)
        {
            foreach (var node in n.GetChildren())
            {
                node.SymTable = n.SymTable;
                node.Accept(this);
            }
        }



        public void Visit(InheritListNode n)
        {
            foreach (var node in n.GetChildren())
            {
                node.SymTable = n.SymTable;
                node.Accept(this);
            }
        }



        public void Visit(TypeNode n)
        {
            foreach (var node in n.GetChildren())
            {
                node.SymTable = n.SymTable;
                node.Accept(this);
            }
        }

        public void Visit(FuncParamListNode n)
        {
            foreach (var node in n.GetChildren())
            {
                node.SymTable = n.SymTable;
                node.Accept(this);
            }
        }

        public void Visit(ArrayDimListNode n)
        {
            foreach (var node in n.GetChildren())
            {
                node.SymTable = n.SymTable;
                node.Accept(this);
            }
        }

        public void Visit(ArrayDimNode n)
        {
            foreach (var node in n.GetChildren())
            {
                node.SymTable = n.SymTable;
                node.Accept(this);
            }
        }

        public void Visit(IntNumNode n)
        {
            foreach (var node in n.GetChildren())
            {
                node.SymTable = n.SymTable;
                node.Accept(this);
            }
        }



        public void Visit(LocalScopeNode n)
        {
            foreach (var node in n.GetChildren())
            {
                node.SymTable = n.SymTable;
                node.Accept(this);
            }
        }



        public void Visit(VarDeclNode n)
        {
            foreach (var node in n.GetChildren())
            {
                node.SymTable = n.SymTable;
                node.Accept(this);
            }
        }




        public void Visit(CompareOpNode n)
        {
            foreach (var node in n.GetChildren())
            {
                node.SymTable = n.SymTable;
                node.Accept(this);
            }
        }









        public void Visit(FuncCallNode n)
        {
            var children = n.GetChildren();

            var first = children.First();
            first.SymTable = n.SymTable;
            first.Accept(this);

            var currentScopeSpec = first.ScopeSpec;
            foreach (var node in children.Skip(1))
            {
                if (string.IsNullOrEmpty(currentScopeSpec))
                {
                    _errorStream.WriteLine($"Use of variable with no scopespec.");
                    break;
                }

                var classTable = _globalTable.GetClassSymbolTableByName(currentScopeSpec);
                if (classTable == null)
                {
                    _errorStream.WriteLine($"ScopeSpec \"{currentScopeSpec}\" refers to a non existing class.");
                    break;
                }

                node.SymTable = classTable;
                node.Accept(this);
                currentScopeSpec = node.ScopeSpec;
            }
        }

        

        



















        public void Visit(VarFuncCallNode n)
        {
            var children = n.GetChildren();

            var first = children.First();
            first.SymTable = n.SymTable;
            first.Accept(this);

            var currentScopeSpec = first.ScopeSpec;
            foreach (var node in children.Skip(1))
            {
                if (!string.IsNullOrEmpty(currentScopeSpec))
                {
                    var classTable = _globalTable.GetClassSymbolTableByName(currentScopeSpec);
                    if (classTable == null)
                    {
                        _errorStream.WriteLine($"ScopeSpec \"{currentScopeSpec}\" refers to a non existing class.");
                        break;
                    }

                    node.SymTable = classTable;
                }
                else
                {
                    node.SymTable = first.SymTable;
                }
                
                node.Accept(this);
                currentScopeSpec = node.ScopeSpec;
            }

            n.ExprType = children.Last().ExprType;
        }

        public void Visit(AssignmentNode n)
        {
            var children = n.GetChildren();

            var rhs = children.Last();
            rhs.SymTable = n.SymTable;
            rhs.Accept(this);
            var rhsType = rhs.ExprType;

            var first = children.First();
            first.SymTable = n.SymTable;
            first.Accept(this);
            var currentScopeSpec = first.ScopeSpec;

            var line = ((IdentifierNode)first.LeftmostChildNode).Token.StartLine;

            for (int i = 1; i < children.Count - 1; ++i)
            {
                if (string.IsNullOrEmpty(currentScopeSpec))
                {
                    _errorStream.WriteLine($"Use of variable with no scopespec.");
                    break;
                }

                var classTable = _globalTable.GetClassSymbolTableByName(currentScopeSpec);
                if (classTable == null)
                {
                    _errorStream.WriteLine($"ScopeSpec \"{currentScopeSpec}\" refers to a non existing class.");
                    break;
                }

                children[i].SymTable = classTable;
                children[i].Accept(this);
                currentScopeSpec = children[i].ScopeSpec;
            }

            var lhsType = children[children.Count - 2].ExprType;

            if (!string.Equals(lhsType.Type, rhsType.Type) || !rhsType.Dims.SequenceEqual(lhsType.Dims))
            {
                _errorStream.WriteLine($"Assignment type missmatch! {lhsType.Type} <-> {rhsType.Type} (line: {line})");
            }
        }


        public void Visit(SubVarCallNode n)
        {
            var children = n.GetChildren();
            foreach (var node in children)
            {
                node.SymTable = n.SymTable;
                node.Accept(this);
            }

            var varNameToken = ((IdentifierNode)children[0]).Token;
            n.VarName = varNameToken.Lexeme;
            n.NumDims = ((IndicesNode)children[1]).NumDims;

            Dictionary<string, (string, List<int>)> varsInScope;
            switch (n.SymTable)
            {
                case FunctionSymbolTableEntry f:
                    {
                        varsInScope = f.GetVariablesInScope()
                                                 .Concat(_globalTable.GetClassSymbolTableByName(f.ScopeSpec)
                                                                     ?.GetVariablesInScope() ?? new Dictionary<string, (string, List<int>)>())
                                                 .ToDictionary(x => x.Key, x => x.Value);
                    }
                    break;
                case ClassSymbolTable s:
                    {
                        varsInScope = s.GetVariablesInScope();
                    }
                    break;
                default:
                    throw new InvalidOperationException("Unknown table entry type found");       
            }

            (string Type, List<int> Dims) typeDims;
            if (!varsInScope.TryGetValue(n.VarName, out typeDims))
            {

                _errorStream.WriteLine($"Use of undeclared variable \"{n.VarName}\" ({varNameToken.StartLine}:{varNameToken.StartColumn})");
            }
            else
            {
                n.ScopeSpec = typeDims.Type;
                n.ExprType = typeDims;

                if (n.NumDims > typeDims.Dims.Count)
                {
                    _errorStream.WriteLine($"Data member \"{n.VarName}\" used with more dims than defined! max: {typeDims.Dims.Count}, had: {n.NumDims}");
                }

                // Say var defined as float a[5][5][5]
                // and used a[2], the type of the expression is now float[5][5]
                n.ExprType = (typeDims.Type, typeDims.Dims.Skip(n.NumDims).ToList());
            }
        }

        public void Visit(SubFuncCallNode n)
        {
            var children = n.GetChildren();

            var token = ((IdentifierNode)children[0]).Token;
            n.FuncName = token.Lexeme;

            List<FunctionSymbolTableEntry> candidateFuncsInScope;
            switch (n.SymTable)
            {
                case FunctionSymbolTableEntry f:
                    {
                        //n.ScopeSpec = f.ScopeSpec;

                        // Same name, in class or free function
                        candidateFuncsInScope = _globalTable.FunctionSymbolTable.Entries
                            .Cast<FunctionSymbolTableEntry>() 
                            .Where(x => string.Equals(x.Name, n.FuncName) && (string.Equals(x.ScopeSpec, f.ScopeSpec) || string.IsNullOrEmpty(x.ScopeSpec)))
                            .ToList();
                    }
                    break;
                case ClassSymbolTable s:
                    {
                        //n.ScopeSpec = s.ClassName;

                        // Same name, in class
                        candidateFuncsInScope = _globalTable.FunctionSymbolTable.Entries
                            .Cast<FunctionSymbolTableEntry>()
                            .Where(x => string.Equals(x.Name, n.FuncName) && (string.Equals(x.ScopeSpec, s.ClassName) || !string.IsNullOrEmpty(x.ScopeSpec)))
                            .ToList();
                    }
                    break;
                default:
                    throw new InvalidOperationException("Unknown table entry type found");
            }

            if (candidateFuncsInScope.Count == 0)
            {
                _errorStream.WriteLine($"Use of undefined function: \"{n.FuncName}\" ({token.StartLine}:{token.StartColumn})");
            }

            // Visit FuncCallParamsNode to get parameter types used
            var funcParams = (FuncCallParamsNode)children[1];
            funcParams.Accept(this);
            var funcParamTypes = funcParams.ParamsTypes;

            FunctionSymbolTableEntry matchingFunc = null;
            foreach (var candidateFunc in candidateFuncsInScope)
            {
                if (candidateFunc.Params.Count != funcParamTypes.Count)
                {
                    continue;
                }

                bool matches = true;
                for (int i = 0; i < candidateFunc.Params.Count; ++i)
                {
                    (string type, List<int> dims) unpacked = funcParamTypes[i];

                    if (!string.Equals(candidateFunc.Params[i].Type.type, unpacked.type))
                    {
                        matches = false;
                        break;
                    }

                    if (!candidateFunc.Params[i].Type.dims.SequenceEqual(unpacked.dims))
                    {
                        matches = false;
                        break;
                    }
                }

                if (matches)
                {
                    matchingFunc = candidateFunc;
                    break;
                }
            }

            if (matchingFunc == null)
            {
                _errorStream.WriteLine($"Use of function: \"{n.FuncName}\" ({token.StartLine}:{token.StartColumn}) with invalid parameters (no matching overload was found)");
            }
            else
            {
                n.ScopeSpec = matchingFunc.ReturnType.Lexeme;
                n.ExprType = (matchingFunc.ReturnType.Lexeme, new List<int>());
            }
        }

        public void Visit(FuncCallParamsNode n)
        {
            var children = n.GetChildren();

            ITable statementsSymTable;
            ASTNodeBase currentNode = n;
            while (!(currentNode is StatementsNode))
            {
                currentNode = currentNode.ParentNode;
                if (currentNode == null)
                {
                    throw new Exception("StatementsNode is somehow not a distant parent of this node...");
                }
            }

            statementsSymTable = currentNode.SymTable;

            foreach (var node in children)
            {
                node.SymTable = statementsSymTable;
                node.Accept(this);
            }

            foreach (var child in children)
            {
                n.ParamsTypes.Add(child.ExprType);
            }
        }

        public void Visit(ExpressionNode n)
        {
            var children = n.GetChildren();
            foreach (var node in children)
            {
                node.SymTable = n.SymTable;
                node.Accept(this);
            }

            if (children.Count == 3)
            {
                n.ExprType = (TypeConstants.BoolType, new List<int>());
            }
            else if (children.Count == 1)
            {
                n.ExprType = children[0].ExprType;
            }
        }

        public void Visit(ArithExprNode n)
        {
            var child = n.GetChildren().Single();

            child.SymTable = n.SymTable;
            child.Accept(this);
            n.ExprType = child.ExprType;
        }




















        public void Visit(IndicesNode n)
        {
            foreach (var node in n.GetChildren())
            {
                node.SymTable = n.SymTable;
                node.Accept(this);
                if (!string.Equals(node.ExprType.Type, TypeConstants.IntType) || node.ExprType.Dims.Count != 0)
                {
                    _errorStream.WriteLine("Expression used in array index must be of int type!");
                }
            }
        }

        

        

        public void Visit(FloatNumNode n)
        {
            foreach (var node in n.GetChildren())
            {
                node.SymTable = n.SymTable;
                node.Accept(this);
            }
        }







        

        public void Visit(VisibilityNode n)
        {
            foreach (var node in n.GetChildren())
            {
                node.SymTable = n.SymTable;
                node.Accept(this);
            }
        }

        public void Visit(MemberDeclNode n)
        {
            foreach (var node in n.GetChildren())
            {
                node.SymTable = n.SymTable;
                node.Accept(this);
            }
        }

        public void Visit(MainFuncNode n)
        {
            foreach (var node in n.GetChildren())
            {
                node.SymTable = _globalTable.FunctionSymbolTable.Entries.Cast<FunctionSymbolTableEntry>().Where(x => string.Equals(x.Name, "main")).Single();
                node.Accept(this);
            }
        }
    }
}