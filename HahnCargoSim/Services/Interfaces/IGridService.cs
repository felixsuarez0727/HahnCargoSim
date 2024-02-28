using HahnCargoSim.Model;

namespace HahnCargoSim.Services.Interfaces
{
  public interface IGridService
  {
    Grid GetGrid();
    string GetGridAsJson();
    List<int>? FindRoute(int startNodeId, int targetNodeId);
    List<int>? FindOptimalRoute(int startNodeId, int targetNodeId);
    int? ConnectionAvailable(int SourceNodeId, int TragetNodeId);
    int GetConnectionCost(int connectionId);
    TimeSpan GetConnectionTime(int connectionId);
    Node GetRandomNode(List<int>? excludedNodes = null);
    Node? GetNode(int nodeId);
    void GenerateFile(int numberOfNodes, int numberOfEdges, int numberOfConnectionsPerNode, string filename);
    int GetGridSizeFactor();


  }
}
