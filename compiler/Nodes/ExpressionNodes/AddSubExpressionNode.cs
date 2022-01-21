﻿using compiler.Interpreter.Visitor;
using compiler.Nodes.Interfaces;
using compiler.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace compiler.Nodes.ExpressionNodes
{
    public class AddSubExpressionNode : IExpressionNode, INode
    {
        public IExpressionNode left;
        public IExpressionNode right;
        public Token operatorToken;

        public AddSubExpressionNode(IExpressionNode left, IExpressionNode right, Token operatorToken)
        {
            this.left = left;
            this.right = right;
            this.operatorToken = operatorToken;
        }

        public void Accept(IVisitor visitor)
        {
            throw new NotImplementedException();
        }
    }
}
