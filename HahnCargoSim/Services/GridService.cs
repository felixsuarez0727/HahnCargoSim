using System.Text.Json;
using HahnCargoSim.Helper;
using HahnCargoSim.Model;
using HahnCargoSim.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace HahnCargoSim.Services
{
  public class GridService : IGridService
  {

    private readonly SimConfig? config;

    private readonly Grid grid;
    private readonly string gridJson;

    public GridService(IOptions<SimConfig> config)
    {
      this.config = config.Value;
      grid = LoadGridFromJson();
      gridJson = LoadGridAsJson();
    }

    public Grid GetGrid()
    {
      return grid;
    }

    public string GetGridAsJson()
    {
      return gridJson;
    }
    public List<int>? FindRoute(int startNodeId, int targetNodeId)
    {
      var visited = new HashSet<int>();
      var route = new List<int>();

      if (DFS(startNodeId, targetNodeId, visited, route))
          return route;
      else
          return null;
    }

    private bool DFS(int currentNodeId, int targetNodeId, HashSet<int> visited, List<int> route)
    {
    
      visited.Add(currentNodeId);
      route.Add(currentNodeId);

      if (currentNodeId == targetNodeId)
          return true;

      foreach (var connection in grid.Connections)
      {
          if (connection.FirstNodeId == currentNodeId && !visited.Contains(connection.SecondNodeId))
          {
              if (DFS(connection.SecondNodeId, targetNodeId, visited, route))
                  return true;
          }
          else if (connection.SecondNodeId == currentNodeId && !visited.Contains(connection.FirstNodeId))
          {
              if (DFS(connection.FirstNodeId, targetNodeId, visited, route))
                  return true;
          }
      }

      route.Remove(currentNodeId);
      return false;
    }
    /*public int? ConnectionAvailable(int sourceNodeId, int targetNodeId)
    {
      return (from connection in grid.Connections where (connection!.FirstNodeId == sourceNodeId && connection.SecondNodeId == targetNodeId) || (connection.FirstNodeId == targetNodeId && connection.SecondNodeId == sourceNodeId) select connection.Id).FirstOrDefault();
    }*/
    public List<int>? FindOptimalRoute(int startNodeId, int targetNodeId)
    {
      var distances = new Dictionary<int, int>(); 
      var previousNodes = new Dictionary<int, int>();
      var queue = new HashSet<int>(); 

      
      foreach (var node in grid.Nodes)
      {
          distances[node.Id] = int.MaxValue;
          previousNodes[node.Id] = -1; 
          queue.Add(node.Id);
      }
      distances[startNodeId] = 0;

      while (queue.Count > 0)
      {
          //Select the node with the minimum distance
          var currentNodeId = queue.OrderBy(nodeId => distances[nodeId]).First();
          queue.Remove(currentNodeId);

          //If we reach the destination node, build and return the optimal route
          if (currentNodeId == targetNodeId)
          {
              var optimalRoute = new List<int>();
              var node = targetNodeId;
              while (node != -1)
              {
                  optimalRoute.Add(node);
                  node = previousNodes[node];
              }
              optimalRoute.Reverse();
              return optimalRoute;
          }

          //Update distances and previous nodes for neighboring nodes of the current node
          foreach (var connection in grid.Connections)
          {
              if (connection.FirstNodeId == currentNodeId || connection.SecondNodeId == currentNodeId)
              {
                  var neighborNodeId = connection.FirstNodeId == currentNodeId ? connection.SecondNodeId : connection.FirstNodeId;
                  var distanceToNeighbor = distances[currentNodeId] + GetConnectionCost(connection.Id);
                  if (distanceToNeighbor < distances[neighborNodeId])
                  {
                      distances[neighborNodeId] = distanceToNeighbor;
                      previousNodes[neighborNodeId] = currentNodeId;
                  }
              }
          }
      }

      return null;
    }

    public int? ConnectionAvailable(int sourceNodeId, int targetNodeId)
    {
        foreach (var connection in grid.Connections)
        {
            if ((connection.FirstNodeId == sourceNodeId && connection.SecondNodeId == targetNodeId) ||
                (connection.FirstNodeId == targetNodeId && connection.SecondNodeId == sourceNodeId))
            {
                return connection.Id;
            }
        }
        
        return null;
    }


    public int GetConnectionCost(int connectionId)
    {
      var con = GetConnection(connectionId);
      Edge? edge = null;
      if (con != null)
      {
        edge = GetEdge(con.EdgeId);
      }
      return edge?.Cost ?? -1;
    }

    public TimeSpan GetConnectionTime(int connectionId)
    {
      var con = GetConnection(connectionId);
      Edge? edge = null;
      if (con != null)
      {
        edge = GetEdge(con.EdgeId);
      }
      return edge?.Time ?? new TimeSpan(0,0,0,0);
    }

    public Node GetRandomNode(List<int>? excludedNodes = null)
    {
      var filteredList = grid.Nodes.ConvertAll(node => node.Clone()).Where(node => excludedNodes == null || !excludedNodes.Contains(node.Id)).ToList();

      var random = new Random();
      return filteredList[random.Next(filteredList.Count)];
    }

    public Node? GetNode(int nodeId)
    {
      return grid.Nodes.Find(node => node.Id == nodeId);
    }

    private Connection? GetConnection(int Id)
    {
      return grid.Connections.Find(c => c != null && c.Id == Id);
    }

    private Edge? GetEdge(int Id)
    {
      return grid.Edges.Find(e => e != null && e.Id == Id);
    }

    private Grid LoadGridFromJson()
    {
      var jsonString = File.ReadAllText(config.GridToLoad);
      return JsonSerializer.Deserialize<Grid>(jsonString)!;
    }

    private string LoadGridAsJson()
    {
      return File.ReadAllText(config.GridToLoad);
    }

    public void GenerateFile(int numberOfNodes, int numberOfEdges, int numberOfConnectionsPerNode, string filename)
    {
      GridGenerator.Run(numberOfNodes, numberOfEdges, numberOfConnectionsPerNode, filename);
    }

    public int GetGridSizeFactor()
    {
      var factor = 1;
      if(grid?.Nodes != null && grid.Nodes.Count > 10)
      {
        factor = grid.Nodes.Count / 10;
      }
      return factor;
    }
  }
}
