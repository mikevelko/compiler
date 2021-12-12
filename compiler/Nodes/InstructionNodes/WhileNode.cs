﻿using compiler.Nodes.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace compiler.Nodes.InstructionNodes
{
    public class WhileNode : IInstructionNode
    {
        public IExpressionNode expressionNode;
        public InstructionsBlockNode instructionsBlockNode;

        public WhileNode(IExpressionNode expressionNode, InstructionsBlockNode instructionsBlockNode)
        {
            this.expressionNode = expressionNode;
            this.instructionsBlockNode = instructionsBlockNode;
        }
    }
}