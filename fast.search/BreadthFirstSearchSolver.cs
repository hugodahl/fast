using System;
using System.Collections.Generic;

namespace fast.search
{
    public class BreadthFirstSearchSolver : ISearchAlgorithm
    {
        public ulong StatesEvaluated { get; private set; }
        public int MaxCostEvaulated { get; private set; }

        public NPuzzle.Location[] Solve(NPuzzle initialState)
        {
            StatesEvaluated = 0UL;
            MaxCostEvaulated = 0;
            
            var openSet = new OpenSet<int, StateCost>();
            var closedSet = new HashSet<NPuzzle>();
            var cameFrom = new Dictionary<NPuzzle, (NPuzzle parent, NPuzzle.Location move)>();
            
            openSet.PushOrImprove(0, new StateCost(initialState, 0));
            cameFrom.Add(initialState, (null, null)); 

            while (!openSet.IsEmpty)
            {
                var stateCost = openSet.PopMin();
                var state = stateCost.State;
                var cost = stateCost.Cost;
                closedSet.Add(state);
                StatesEvaluated++;
                MaxCostEvaulated = Math.Max(MaxCostEvaulated, cost);
                if (state.IsGoal()) return RebuildSolution(cameFrom, state);
                foreach(var move in state.ExpandMoves())
                {
                    var successor = state.MoveCopy(move);
                    if (closedSet.Contains(successor)) continue;
                    // why is this 1 and not step cost? because that's how we enforce
                    // the BFS property of exploring on level fully before starting the next
                    openSet.PushOrImprove(cost + 1, new StateCost(successor, cost + 1));
                    cameFrom[successor] = (state, move);
                }
            }
            return null;
        }

        private NPuzzle.Location[] RebuildSolution(
            Dictionary<NPuzzle, (NPuzzle parent, NPuzzle.Location move)> cameFrom,
            NPuzzle end
        )
        {
            var state = end;
            var solution = new List<NPuzzle.Location>();
            while(true)
            {
                var (parent, move) = cameFrom[state];
                if (move == null) break;
                solution.Add(move);
                state = parent;
            }
            solution.Reverse();
            return solution.ToArray();
        }

        public override string ToString()
        {
            return "Breadth First Search (BFS)";
        }
    }
}