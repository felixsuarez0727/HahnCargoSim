# README

## Description:
This method, FindOptimalRoute, has been created to find the optimal route. It utilizes Dijkstra's algorithm to efficiently compute the shortest path.

### Method Signature:
public List<int>? FindOptimalRoute(int startNodeId, int targetNodeId)

### Parameters:
- startNodeId: The ID of the starting node.
- targetNodeId: The ID of the target node.

### Return:
- If a route is found, it returns a List<int> containing the optimal route from the start node to the target node.
- If no route is found, it returns null.

### Usage:
This method is implemented to facilitate obtaining the optimal route for cargo transportation.


## Additional Information:
- This method is part of a cargo transporter system where the endpoint /CargoTransporter/Move initiates cargo movement.
