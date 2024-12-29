using Core.Common.DataStructures;

namespace Tests.Core.Common
{
    public class GraphUnitTests
    {
        [Fact]
        public void MinimizeEdges_SimpleGraph_MinimizesCorrectly()
        {
            var graph = new Graph<int>();

            var personA = Guid.NewGuid();
            var personB = Guid.NewGuid();
            var personC = Guid.NewGuid();

            graph.SetValue(personA, personB, 15);
            graph.SetValue(personB, personC, 5);
            graph.SetValue(personC, personA, 7);

            graph.MinimizeEdges();

            Assert.Equal(8, graph.GetValue(personA, personB));
            Assert.Equal(0, graph.GetValue(personB, personC));
            Assert.Equal(0, graph.GetValue(personC, personA));
        }

        [Fact]
        public void MinimizeEdges_ComplexGraph_MinimizesCorrectly()
        {
            var graph = new Graph<int>();

            var personA = Guid.NewGuid();
            var personB = Guid.NewGuid();
            var personC = Guid.NewGuid();
            var personD = Guid.NewGuid();

            graph.SetValue(personA, personB, 20);
            graph.SetValue(personB, personC, 10);
            graph.SetValue(personC, personD, 5);
            graph.SetValue(personD, personA, 15);

            graph.MinimizeEdges();

            Assert.Equal(5, graph.GetValue(personA, personC));
            Assert.Equal(10, graph.GetValue(personD, personB));
        }

        [Fact]
        public void MinimizeEdges_NoDebts_NoChanges()
        {
            var graph = new Graph<int>();

            var personA = Guid.NewGuid();
            var personB = Guid.NewGuid();

            graph.SetValue(personA, personB, 0);

            graph.MinimizeEdges();

            Assert.Equal(0, graph.GetValue(personA, personB));
        }

        [Fact]
        public void MinimizeEdges_VertexesHasTwoEdgesBetween()
        {
            var graph = new Graph<int>();

            var personA = Guid.NewGuid();
            var personB = Guid.NewGuid();

            graph.SetValue(personA, personB, 10);
            graph.SetValue(personB, personA, 5);

            graph.MinimizeEdges();

            Assert.Equal(5, graph.GetValue(personA, personB));
        }

        [Fact]
        public void MinimizeEdges_ShouldHandleModifiedValuesCorrectly()
        {
            var graph = new Graph<int>();
            var controlGraph = new Graph<int>();
            var personA = Guid.NewGuid();
            var personB = Guid.NewGuid();
            var personC = Guid.NewGuid();

            graph.SetValue(personA, personB, 20);
            graph.SetValue(personB, personC, 10);
            graph.SetValue(personC, personA, 5);

            graph.MinimizeEdges();

            graph.SetValue(personB, personC, 20);

            graph.MinimizeEdges();

            controlGraph.SetValue(personA, personB, 20);
            controlGraph.SetValue(personB, personC, 30);
            controlGraph.SetValue(personC, personA, 5);

            controlGraph.MinimizeEdges();

            Assert.Equal(controlGraph.GetValue(personA, personB), graph.GetValue(personA, personB));
            Assert.Equal(controlGraph.GetValue(personB, personC), graph.GetValue(personB, personC));
            Assert.Equal(controlGraph.GetValue(personC, personA), graph.GetValue(personC, personA));
        }

        [Fact]
        public void MinimizeEdges_ComplexGraph_MinimizesCorrectlyExampleFromThesis()
        {
            var graph = new Graph<int>();

            var personA = Guid.NewGuid();
            var personB = Guid.NewGuid();
            var personC = Guid.NewGuid();
            var personD = Guid.NewGuid();

            graph.SetValue(personA, personB, 10);
            graph.SetValue(personB, personC, 15);
            graph.SetValue(personC, personD, 20);
            graph.SetValue(personD, personA, 25);
            graph.SetValue(personD, personB, 5);
            graph.SetValue(personC, personA, 5);

            graph.MinimizeEdges();

            Assert.Equal(10, graph.GetValue(personC, personA));
            Assert.Equal(10, graph.GetValue(personD, personA));
        }

        [Fact]
        public void HasCycle_ShouldReturnFalse_WhenMatrixIsAcyclic()
        {
            var matrix = new Graph<int>();
            Guid node1 = Guid.NewGuid();
            Guid node2 = Guid.NewGuid();
            Guid node3 = Guid.NewGuid();

            matrix.SetValue(node1, node2, 1);
            matrix.SetValue(node2, node3, 1);

            bool result = matrix.HasCycle();

            Assert.False(result);
        }

        [Fact]
        public void HasCycle_ShouldReturnTrue_WhenMatrixHasSimpleCycle()
        {
            var matrix = new Graph<int>();
            Guid node1 = Guid.NewGuid();
            Guid node2 = Guid.NewGuid();
            Guid node3 = Guid.NewGuid();

            matrix.SetValue(node1, node2, 1);
            matrix.SetValue(node2, node3, 1);
            matrix.SetValue(node3, node1, 1);

            bool result = matrix.HasCycle();

            Assert.True(result);
        }

        [Fact]
        public void HasCycle_ShouldReturnFalse_WhenMatrixIsEmpty()
        {
            var matrix = new Graph<int>();

            bool result = matrix.HasCycle();

            Assert.False(result);
        }

        [Fact]
        public void HasCycle_ShouldReturnTrue_WhenMatrixHasSelfLoop()
        {
            var matrix = new Graph<int>();
            Guid node1 = Guid.NewGuid();

            matrix.SetValue(node1, node1, 1);

            bool result = matrix.HasCycle();

            Assert.True(result);
        }

        [Fact]
        public void HasCycle_ShouldReturnTrue_WhenMatrixHasMultipleCycles()
        {
            var matrix = new Graph<int>();
            Guid node1 = Guid.NewGuid();
            Guid node2 = Guid.NewGuid();
            Guid node3 = Guid.NewGuid();
            Guid node4 = Guid.NewGuid();

            matrix.SetValue(node1, node2, 1);
            matrix.SetValue(node2, node3, 1);
            matrix.SetValue(node3, node1, 1);
            matrix.SetValue(node3, node4, 1);
            matrix.SetValue(node4, node3, 1);

            bool result = matrix.HasCycle();

            Assert.True(result);
        }
    }
}
