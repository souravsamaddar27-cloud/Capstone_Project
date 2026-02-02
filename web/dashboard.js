console.log('dashboard.js loaded');

// Chart refs for cleanup
let spxLaunchesChart, spxSuccessChart, cgTopCoinsChart, cgDistChart, omHourlyChart;
let usersChartInstance, todosChartInstance;

// Hold the fetched summary (for reset) and current summary in memory
let fetchedSummary = null;
let currentSummary = null;

// Utility: destroy a Chart.js instance safely
function destroyChart(instance) { try { if (instance) instance.destroy(); } catch (_) { } }

// Utility: show/hide sections (multi vs single)
function showSections({ multi, single }) {
    const spaceX = document.getElementById('spaceXSection');
    const coinGecko = document.getElementById('coinGeckoSection');
    const openMeteo = document.getElementById('openMeteoSection');
    const singleApi = document.getElementById('singleApiSection');

    if (spaceX) spaceX.hidden = !multi;
    if (coinGecko) coinGecko.hidden = !multi;
    if (openMeteo) openMeteo.hidden = !multi;
    if (singleApi) singleApi.hidden = !single;
}

// Utility: show only one of the multi sections
function showOnlySection(id) {
    const ids = ['spaceXSection', 'coinGeckoSection', 'openMeteoSection'];
    ids.forEach(sid => {
        const el = document.getElementById(sid);
        if (el) el.hidden = (sid !== id);
    });
}

// Initialize UI from a summary object
function initFromSummary(summary) {
    currentSummary = summary;

    // Timestamp
    const tsEl = document.getElementById('lastUpdated');
    if (tsEl) tsEl.textContent = 'Last updated: ' + new Date().toLocaleString();

    // Detect data shapes
    const hasSpaceX = !!summary && !!summary.SpaceX;
    const hasCoinGecko = !!summary && !!summary.CoinGecko;
    const hasOpenMeteo = !!summary && !!summary.OpenMeteo;
    const hasMulti = hasSpaceX || hasCoinGecko || hasOpenMeteo;

    const hasSingleTopUsers = Array.isArray(summary?.topUsers);
    const hasSingleMostCommented = Array.isArray(summary?.mostCommentedPosts);
    const hasSingleTodos = !!summary?.todos && typeof summary.todos.completed === 'number' && typeof summary.todos.pending === 'number';
    const hasSingle = hasSingleTopUsers || hasSingleMostCommented || hasSingleTodos;

    const select = document.getElementById('sourceSelect');
    if (!select) return;
    select.innerHTML = '';

    if (hasMulti) {
        showSections({ multi: true, single: false });

        // Build source list
        const sources = [];
        if (hasSpaceX) sources.push({ key: 'SpaceX', section: 'spaceXSection' });
        if (hasCoinGecko) sources.push({ key: 'CoinGecko', section: 'coinGeckoSection' });
        if (hasOpenMeteo) sources.push({ key: 'OpenMeteo', section: 'openMeteoSection' });

        sources.forEach(s => {
            const opt = document.createElement('option');
            opt.value = s.key;
            opt.textContent = s.key;
            select.appendChild(opt);
        });

        // Initial render and section visibility
        renderSource(summary, sources[0].key);
        showOnlySection(sources[0].section);

        // Change handler
        select.onchange = e => {
            const key = e.target.value;
            renderSource(summary, key);
            const sec = sources.find(s => s.key === key)?.section;
            showOnlySection(sec);
        };
    } else if (hasSingle) {
        showSections({ multi: false, single: true });
        const opt = document.createElement('option');
        opt.value = 'Single';
        opt.textContent = 'Single API';
        select.appendChild(opt);
        select.onchange = null;
        renderSingle(summary);
    } else {
        // No recognizable data
        showSections({ multi: false, single: false });
        const opt = document.createElement('option');
        opt.value = '';
        opt.textContent = 'No sources available';
        select.appendChild(opt);
        console.warn('No multi or single API data detected in summary.json.');
    }
}

// Dispatcher for multi-API renderers
function renderSource(summary, key) {
    ['spxNoData', 'cgNoData', 'omNoData'].forEach(id => {
        const el = document.getElementById(id);
        if (el) el.hidden = true;
    });
    if (key === 'SpaceX') renderSpaceX(summary.SpaceX);
    if (key === 'CoinGecko') renderCoinGecko(summary.CoinGecko);
    if (key === 'OpenMeteo') renderOpenMeteo(summary.OpenMeteo);
}

// SpaceX rendering
function renderSpaceX(spx) {
    const noDataEl = document.getElementById('spxNoData');
    const launchesCanvas = document.getElementById('spxLaunchesChart');
    const successCanvas = document.getElementById('spxSuccessChart');

    if (!spx || !Array.isArray(spx.launchesPerYear) || spx.launchesPerYear.length === 0) {
        if (noDataEl) noDataEl.hidden = false;
    } else {
        if (noDataEl) noDataEl.hidden = true;
        destroyChart(spxLaunchesChart);
        const labels = spx.launchesPerYear.map(x => x.year);
        const values = spx.launchesPerYear.map(x => x.count);
        spxLaunchesChart = new Chart(launchesCanvas.getContext('2d'), {
            type: 'bar',
            data: { labels, datasets: [{ label: 'Launches', data: values, backgroundColor: 'rgba(54,162,235,0.6)' }] },
            options: { responsive: true, scales: { y: { beginAtZero: true, ticks: { precision: 0 } } } }
        });
    }

    destroyChart(spxSuccessChart);
    const rate = (typeof spx?.successRate === 'number') ? spx.successRate : 0;
    spxSuccessChart = new Chart(successCanvas.getContext('2d'), {
        type: 'doughnut',
        data: {
            labels: ['Success', 'Failure'],
            datasets: [{
                data: [Math.round(rate * 100), Math.round(100 - rate * 100)],
                backgroundColor: ['#4caf50', '#f44336'],
                borderColor: '#ffffff',
                borderWidth: 2
            }]
        },
        options: { responsive: true, cutout: '60%', plugins: { legend: { position: 'bottom' } } }
    });
}

// CoinGecko rendering (will only render if CoinGecko data exists)
function renderCoinGecko(cg) {
    const noDataEl = document.getElementById('cgNoData');
    const topCanvas = document.getElementById('cgTopCoinsChart');
    const distCanvas = document.getElementById('cgDistChart');

    if (!cg || !Array.isArray(cg.topCoins) || cg.topCoins.length === 0) {
        if (noDataEl) noDataEl.hidden = false;
    } else {
        if (noDataEl) noDataEl.hidden = true;
        destroyChart(cgTopCoinsChart);
        const labels = cg.topCoins.map(x => x.name);
        const values = cg.topCoins.map(x => x.value);
        cgTopCoinsChart = new Chart(topCanvas.getContext('2d'), {
            type: 'bar',
            data: { labels, datasets: [{ label: 'Market Cap', data: values, backgroundColor: 'rgba(255,159,64,0.6)' }] },
            options: { responsive: true, scales: { y: { beginAtZero: true } } }
        });
    }

    destroyChart(cgDistChart);
    const gainers = (typeof cg?.change24hDistribution?.gainers === 'number') ? cg.change24hDistribution.gainers : 0;
    const losers = (typeof cg?.change24hDistribution?.losers === 'number') ? cg.change24hDistribution.losers : 0;
    cgDistChart = new Chart(distCanvas.getContext('2d'), {
        type: 'pie',
        data: { labels: ['Gainers', 'Losers'], datasets: [{ data: [gainers, losers], backgroundColor: ['#4caf50', '#f44336'] }] },
        options: { responsive: true, plugins: { legend: { position: 'bottom' } } }
    });
}

// OpenMeteo rendering
function renderOpenMeteo(om) {
    const noDataEl = document.getElementById('omNoData');
    const hourlyCanvas = document.getElementById('omHourlyChart');
    const avgEl = document.getElementById('omAvgTemp');

    if (!om || !Array.isArray(om.hourlyTemp) || om.hourlyTemp.length === 0) {
        if (noDataEl) noDataEl.hidden = false;
        if (avgEl) avgEl.textContent = '—';
    } else {
        if (noDataEl) noDataEl.hidden = true;
        destroyChart(omHourlyChart);
        const labels = om.hourlyTemp.map(x => x.time);
        const values = om.hourlyTemp.map(x => x.value);
        omHourlyChart = new Chart(hourlyCanvas.getContext('2d'), {
            type: 'line',
            data: { labels, datasets: [{ label: 'Temp (°C)', data: values, borderColor: '#1565c0', backgroundColor: 'rgba(21,101,192,0.2)', tension: 0.2 }] },
            options: { responsive: true, plugins: { legend: { position: 'bottom' } }, scales: { y: { beginAtZero: false } } }
        });

        const avg = (typeof om.avgTemp === 'number') ? om.avgTemp : 0;
        if (avgEl) avgEl.textContent = `${avg.toFixed(1)} °C`;
    }
}

// Single-API rendering (fallback)
function renderSingle(summary) {
    const topUsers = Array.isArray(summary?.topUsers) ? summary.topUsers : [];
    const posts = Array.isArray(summary?.mostCommentedPosts) ? summary.mostCommentedPosts : [];
    const todos = (summary?.todos && typeof summary.todos.completed === 'number' && typeof summary.todos.pending === 'number')
        ? summary.todos
        : { completed: 0, pending: 0 };

    const noDataEl = document.getElementById('singleNoData');

    // Users chart
    const uCanvas = document.getElementById('usersChart');
    destroyChart(usersChartInstance);
    if (uCanvas && topUsers.length > 0) {
        const labels = topUsers.map(u => String(u.name || ''));
        const values = topUsers.map(u => Number(u.postCount || 0));
        usersChartInstance = new Chart(uCanvas.getContext('2d'), {
            type: 'bar',
            data: { labels, datasets: [{ label: 'Posts', data: values, backgroundColor: 'rgba(54,162,235,0.6)' }] },
            options: { responsive: true, scales: { y: { beginAtZero: true, ticks: { precision: 0 } } } }
        });
    }

    // Todos chart
    const tCanvas = document.getElementById('todosChart');
    destroyChart(todosChartInstance);
    if (tCanvas) {
        const completed = Number(todos.completed || 0);
        const pending = Number(todos.pending || 0);
        todosChartInstance = new Chart(tCanvas.getContext('2d'), {
            type: 'pie',
            data: { labels: ['Completed', 'Pending'], datasets: [{ data: [completed, pending], backgroundColor: ['#4caf50', '#f44336'] }] },
            options: { responsive: true, plugins: { legend: { position: 'bottom' } } }
        });
    }

    // Most commented posts list
    const list = document.getElementById('topPosts');
    if (list) {
        list.innerHTML = '';
        if (posts.length > 0) {
            posts.forEach(p => {
                const li = document.createElement('li');
                li.textContent = `${String(p.title || '')} (${Number(p.commentCount || 0)} comments)`;
                list.appendChild(li);
            });
        }
    }

    const noData = (topUsers.length === 0 && posts.length === 0 && (!todos || (todos.completed === 0 && todos.pending === 0)));
    if (noDataEl) noDataEl.hidden = !noData;
}

//// Fetch summary.json (with cache-buster). If it fails, upload still works.
//function fetchSummary() {
//    return fetch('summary.json?ts=' + Date.now())
//        .then(r => {
//            console.log('Fetch status:', r.status);
//            if (!r.ok) throw new Error('Could not load summary.json');
//            return r.json();
//        });
//}

const REFRESH_MS = 15000; // 15 seconds
console.log('Auto-refresh set to', REFRESH_MS, 'ms');

// Refresh function: fetch summary.json and render
function refresh() {
    console.log('Refreshing at', new Date().toLocaleTimeString());
    return fetch('summary.json?ts=' + Date.now())
        .then(r => {
            console.log('Fetch status:', r.status);
            if (!r.ok) throw new Error('Could not load summary.json');
            return r.json();
        })
        .then(summary => {
            console.log('SUMMARY DATA (fetched):', summary);
            fetchedSummary = summary;     // keep a copy for "Reset" button
            initFromSummary(summary);     // render
        })
        .catch(err => {
            console.error('Error loading dashboard data:', err);
            // Optional: show a small note or keep silent and rely on upload feature
        });
}

// Call once on page load, then every 15s
refresh();
setInterval(refresh, REFRESH_MS);

// Upload/reset listeners (attach once)
const fileInput = document.getElementById('fileInput');
if (fileInput) {
    fileInput.addEventListener('change', async (e) => {
        const file = e.target.files && e.target.files[0];
        if (!file) return;
        try {
            const text = await file.text();
            const summary = JSON.parse(text);
            console.log('SUMMARY DATA (uploaded):', summary);
            initFromSummary(summary);
        } catch (ex) {
            console.error('Error parsing uploaded JSON:', ex);
            alert('Invalid JSON file. Please select a valid summary.json.');
        }
    });
}

const resetBtn = document.getElementById('resetBtn');
if (resetBtn) {
    resetBtn.addEventListener('click', () => {
        if (!fetchedSummary) {
            alert('No fetched data available yet.');
            return;
        }
        console.log('Resetting to fetched data.');
        initFromSummary(fetchedSummary);
    });
}
