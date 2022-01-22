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
        public void Visit(ParametersListNode parametersListNode);
        public void Visit(ReturnNode returnNode);



        //Expressions
        public void Visit(SimpleIntNode simpleIntNode);
        public void Visit(SimpleDoubleNode simpleDoubleNode);
        public void Visit(SimpleIdentifierNode simpleIdentifierNode);

        public void Visit(AndExpressionNode andExpressionNode);
        public void Visit(OrExpressionNode orExpressionNode);
        public void Visit(LogicNegationExpressionNode logicNegationExpressionNode);
        public void Visit(ComparisonExpressionNode comparisonExpressionNode);
        public void Visit(AddSubExpressionNode addSubExpressionNode);
        public void Visit(MulDivExpressionNode mulDivExpressionNode);
        public void Visit(UnaryExpressionNode unaryExpressionNode);
        

        public void Visit(VarAssignmentOrFuncInvocationNode varAssignmentOrFuncInvocationNode);
    }
}
