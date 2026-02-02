Architecture Overview

	This project implements a modular, layered architecture to fetch public API data, process insights via LINQ, output a JSON summary, and visualize the results in a static dashboard.
		
		Backend: C# console app (ApiHarvester) targeting .NET Framework 4.7.2
		Frontend: Static HTML/CSS/JS using Chart.js
		API: JSONPlaceholder (posts, users, comments, todos)
		Serialization: Newtonsoft.Json
		Patterns: Dependency Injection (via interfaces), optional Singleton for config, SOLID

Goals

	Fetch ≥100 records asynchronously
	Aggregate insights with LINQ
	Generate summary.json for a static dashboard
	Keep code modular, testable, and easy to extend

Tech Stack

	C#: .NET Framework 4.7.2
	HttpClient with async/await
	Newtonsoft.Json for serialization/deserialization
	Chart.js for visualizations
	Visual Studio (or CLI) for build/run

High-Level Architecture

	ApiClient (Service)
		Fetches data from JSONPlaceholder endpoints asynchronously (posts/users/comments/todos)
		Error handling, timeouts, simple retries
	DataProcessor (Service)
		LINQ analytics (grouping, counting, joining)
		Generates Top Users, Most Commented Posts, and Todos completion stats
	ReportWriter (Service)
		Serializes a SummaryReport and writes summary.json to web/ folder
	Dashboard (Frontend)
		index.html + styles.css + dashboard.js
		fetches summary.json and renders charts

Data Flow

	Plaintext diagram: API → ApiClient → DataProcessor → ReportWriter → summary.json → Dashboard (Chart.js)

	Detail:
		ApiClient: GET /posts, /users, /comments, /todos
		DataProcessor: LINQ group/aggregate (Top Users, Most Commented, Todos stats)
		ReportWriter: SummaryReport → JSON → web/summary.json
		Dashboard: fetch('summary.json') → Chart.js renders Bar/List/Pie (or Donut)

Module Decomposition

	Models/
		Post, User, Comment, Todo
		SummaryReport (topUsers, mostCommentedPosts, todos)
	Services/
		IApiClient / ApiClient
		IDataProcessor / DataProcessor
		IReportWriter / ReportWriter
	Tests/ (test harness within the app or separate test project)
		Assert helper
		DataProcessorTests, ApiClientTests (mocked HttpClient)

Data Contracts

	summary.json schema (consumed by dashboard):

	topUsers: array of
		name: string
		postCount: number
	mostCommentedPosts: array of
		title: string
		commentCount: number
	todos: object
		completed: number
		pending: number
	Example: { "topUsers": [{ "name": "User A", "postCount": 10 }], "mostCommentedPosts": [{ "title": "Post 1", "commentCount": 15 }], "todos": { "completed": 75, "pending": 25 } }

Dependency Injection (DI)

	The Program orchestrates via interfaces:
	IApiClient api = new ApiClient();
	IDataProcessor processor = new DataProcessor();
	IReportWriter writer = new ReportWriter();
	The ApiClient also supports an injectable HttpClient for testing/mocking.

Optional Singleton (Config)

	If needed, a simple AppConfig singleton (e.g., ApiBaseUrl) can centralize configuration.
	Justification: consistent source of truth; one instance across the app.

SOLID Principles

	Single Responsibility:
		ApiClient only fetches data
		DataProcessor only computes insights
		ReportWriter only writes JSON
	Open/Closed:
		Add new analytics or endpoints without modifying existing service contracts
	Liskov Substitution:
		Interfaces (IApiClient, IDataProcessor, IReportWriter) allow swapping implementations
	Interface Segregation:
		Small focused interfaces instead of one large one
	Dependency Inversion:
		Program depends on abstractions (interfaces), not concrete classes

Error Handling Strategy

	ApiClient:
		Timeout (10s), retries with backoff (3 attempts), checks response.IsSuccessStatusCode
		Catches TaskCanceledException, HttpRequestException, JsonException
	DataProcessor:
		Defensive coding: handles null inputs, returns non-null lists
	Dashboard:
		fetch error handling + user-friendly message
		basic safe defaults (optional) to avoid broken renders

Testing Strategy

	DataProcessorTests: verifies LINQ grouping/aggregation
	ApiClientTests: fakes HttpMessageHandler, deserialization correctness
	Test harness can be run via: ApiHarvester.exe --run-tests

Build & Run (Backend)

	Build solution in Visual Studio
	Run ApiHarvester (no args) to generate web/summary.json
	Confirm file at: web/summary.json

Serve Dashboard

	In a terminal:
	cd <solution>\web
	python -m http.server 8080
	Visit: http://localhost:8080
	Hard refresh if needed (Ctrl+F5)

Limitations & Assumptions

	JSONPlaceholder returns static data (e.g., 10 posts per user)
	For demos, optional flag can randomize counts in Program without changing core processing
	summary.json contract must match dashboard expectations

Future Enhancements

	Add more analytics (e.g., time-based trends, per-user todo completion)
	Multiple data sources/APIs
	Persist summaries; schedule periodic refresh
	CI to run tests and static analysis on each change
	Switch to .NET 6+ in future for modern features

