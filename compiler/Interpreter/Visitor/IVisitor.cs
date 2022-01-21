using compiler.Nodes;
using compiler.Nodes.ExpressionNodes;
using compiler.Nodes.ExpressionNodes.SimpleExpressionNodes;
using compiler.Nodes.InstructionNodes;
using compiler.Nodes.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace compiler.Interpreter.Visitor
{
    public interface IVisitor
    {
        public void Visit(ProgramNode programNode);
        public void Visit(FunctionNode functionNode);
        public void Visit(InstructionsBlockNode instructionsBlockNode);

        public void Visit(InstructionsListNode instructionsListNode);
        
        //Instructions
        public void Visit(VariableDefinitionNode variableDefinitionNode);

        public void Visit(IfNode ifNode);

        public void Visit(ElseNode elseNode);
        public void Visit(WhileNode whileNode);
        public void Visit(IdentifierAssignmentOrInvocationNode identifierAssignmentOrInvocationNode);

        //Expressions
        public void Visit(SimpleIntNode simpleIntNode);

        public void Visit(ComparisonExpressionNode comparisonExpressionNode);

        public void Visit(VarAssignmentOrFuncInvocationNode varAssignmentOrFuncInvocationNode);
    }
}
