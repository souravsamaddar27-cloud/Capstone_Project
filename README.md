## Documentation

- Architecture: [docs/architecture.md](docs/architecture.md)
- Performance Notes: [docs/perf.md](docs/perf.md)

Optional: link an image in docs

If you export a diagram as docs/architecture.png:
markdown

![Architecture Diagram](docs/architecture.png)

## Architecture Overview

This project follows a **modular, layered architecture**:

- **Data Model Layer:** Defines data structures for API data.
- **API Service Layer:** Handles fetching of data from external APIs (JSONPlaceholder).
- **Processing Layer:** Contains LINQ logic for data aggregation and analytics.
- **Reporting Layer:** Serializes processed data into a summary JSON file for the dashboard.
- **Presentation Layer:** A static web dashboard (HTML/JS/Chart.js) that reads and visualizes the summary data.

**Rationale:**  
This architecture ensures clear separation of concerns, maintainability, and testability.  
MVC/MVVM patterns are not directly applied, as the project does not involve interactive server-side web UI or data-bound client frameworks.

**Diagram:**

+---------------------+          +-----------------+          +-----------------+
|   API Fetcher       |  --->    |  Data Processor |  --->    |  Report Writer  |
| (IApiClient, etc.)  |          | (LINQ, etc.)    |          | (summary.json)  |
+---------------------+          +-----------------+          +-----------------+
         ^                                                             |
         |                                                             v
         |                                                     +---------------+
         |                                                     |   Dashboard   |
         |                                                     | (HTML/JS/CSS) |
         +---------------------------------------------------> |  Chart.js     |
                                                               +---------------+

Visual Flow:
[ApiClient.cs] ----(HTTP GET)----> [JSONPlaceholder API]
      |                                |
      |<---(JSON Response)-------------|
      |
[LINQ Processing] -> [summary.json] -> [Web Dashboard]

**For UNIT TESTING follow the following:
PS C:\Users\srivar17\source\repos\ApiHarvesterSolution> cd ApiHarvester
PS C:\Users\srivar17\source\repos\ApiHarvesterSolution\ApiHarvester> cd bin
PS C:\Users\srivar17\source\repos\ApiHarvesterSolution\ApiHarvester\bin> cd Debug
PS C:\Users\srivar17\source\repos\ApiHarvesterSolution\ApiHarvester\bin\Debug> .\ApiHarvester.exe --run-tests