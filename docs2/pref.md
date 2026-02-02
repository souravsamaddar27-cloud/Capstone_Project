Performance & Reliability Notes

This document outlines performance goals, measurements, and optimizations for the backend pipeline and the dashboard.

Objectives

	Fetch ≥100 records asynchronously
	Keep fetch operations reliable (timeouts, retries)
	Efficient in-memory aggregation with LINQ
	Generate summary.json promptly and serve it quickly
	Keep the dashboard responsive and cache-safe

Environment
	OS: Windows 10
	.NET Framework: 4.7.2
	Tooling: Visual Studio
	API: JSONPlaceholder
	Frontend server: simple static server (Python http.server)

Workload
	API endpoints and typical record counts:

	/posts → ~100
	/users → ~10
	/comments → ~500
	/todos → ~200

Backend Performance Design
	Async/await with HttpClient
		Non-blocking I/O for all HTTP calls
		10s timeout to prevent hangs
	Simple retry policy
		Up to 3 attempts, small backoff (2s…5s)
		Handles transient failures
	JSON deserialization
		Newtonsoft.Json; fast for typical payload sizes
	LINQ aggregation
		Complexity: O(n) for single pass groupings per collection
		Minimal allocations beyond grouping and projections
	File output
		Summary written once to web/summary.json
		Pretty-printed JSON for ease of inspection (can be compact if needed)


Suggested Measurements (How-To)
	Use Stopwatch to measure durations; log counts and timings.

	Example instrumentation (pseudo):
		Fetch times:
			posts/users/comments/todos fetch duration (ms)
		Processing time:
			DataProcessor.GenerateSummary duration (ms)
		Write time:
			ReportWriter.WriteSummary duration (ms)
	
	Expected baseline (fill after measuring):
		Fetch:
			posts: ~XXX ms, users: ~XXX ms, comments: ~XXX ms, todos: ~XXX ms
		Processing: ~XXX ms
		Write JSON: ~XX ms
		Total end-to-end: ~XXX ms

Parallel Fetch (Optional)
	You can start all fetches, then await with Task.WhenAll to reduce total latency:
		var postsTask = api.FetchPostsAsync(); etc.
		await Task.WhenAll(postsTask, usersTask, commentsTask, todosTask);
	Note: Keep retry/timeout handling; parallelizing reduces wall-clock time.

Reliability Considerations
	Timeouts and retries prevent unbounded waits and absorb transient errors
	Check response.IsSuccessStatusCode before reading
	Validate deserialization result is non-null; throw/handle if null
	Defensive DataProcessor (null → empty lists)

Serialization & File I/O
	Newtonsoft.Json with Formatting.Indented (dev friendly)
	For maximal throughput:
		Consider omitting indentation in production
		Ensure the write path is local and correct
		Wrap write in try/catch to log I/O errors

LINQ Efficiency
	GroupBy + Count are O(n) over each collection
	Join operations rely on hash-based equality for lookups (efficient for typical in-memory sizes)
	Avoid multiple passes; compute only what you visualize

Frontend Performance
	Cache busting: fetch('summary.json?ts=' + Date.now()) to avoid stale responses
	Chart.js rendering:
		Keep datasets small for snappy redraws
		Destroy previous Chart instances before rerendering to prevent memory leaks
	Responsive layout via CSS; minimal JS work at load time

Bottlenecks & Optimizations (Checklist)
	If fetching dominates:
		Parallelize fetches with Task.WhenAll
		Increase backoff for 429 / explore Retry-After headers
	If processing dominates:
		Ensure single-pass groupings; avoid redundant ToList calls
	If serialization dominates:
		Remove indentation; reuse serializer settings
	If dashboard stalls:
		Limit list items; lazy render long lists
		Ensure reduced chart animations if needed

Results (Fill In After Running)
	Fetch timings (ms):
		Posts: __
		Users: __
		Comments: __
		Todos: __
	Processing (ms): __
	JSON write (ms): __
	End-to-end (ms): __
	Error rate (%) over N runs: __

Action Items
	 Add Stopwatch-based logging around fetch/process/write
	 Optionally enable Task.WhenAll for parallel fetches
	 Consider compact JSON in production
	 Add CI to run tests and basic static analysis on push

Known Limitations
	JSONPlaceholder data is static; insights don’t change unless randomized or a different API is used
	Network conditions directly affect fetch timings
	.NET Framework 4.7.2 limits access to some newer C# features (addressed with LangVersion where needed)