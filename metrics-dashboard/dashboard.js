console.log("dashboard.js loaded");
const API_URL = "http://localhost:5155/api/metrics/summary";
const REFRESH_MS = 15000;

async function loadMetrics() {
    try {
        const response = await fetch(API_URL);
        if (!response.ok) throw new Error("API error");

        const data = await response.json();

        renderTable(data.filter(x => x.activityType === 1), "telemetryTable");
        renderTable(data.filter(x => x.activityType === 2), "apiTable");

    } catch (err) {
        console.error("Failed to load metrics:", err);
    }
}

function renderTable(rows, tableId) {
    const tbody = document.querySelector(`#${tableId} tbody`);
    tbody.innerHTML = "";

    rows.forEach(r => {
        const tr = document.createElement("tr");

        tr.innerHTML = `
            <td>${r.subActivityName}</td>
            <td>${r.averageExecutionMs.toFixed(2)}</td>
            <td>${r.minExecutionMs}</td>
            <td>${r.maxExecutionMs}</td>
            <td>${r.averageMemoryBytes.toFixed(0)}</td>
            <td>${r.minMemoryBytes}</td>
            <td>${r.maxMemoryBytes}</td>
        `;

        tbody.appendChild(tr);
    });
}
async function loadApiHarvesterSummary() {
    try {
        const response = await fetch("summary.json?ts=" + Date.now());
        if (!response.ok) throw new Error("summary.json not found");

        const data = await response.json();

        console.log("API Harvester data loaded:", data);

        renderApiHarvester(data);

    } catch (err) {
        console.error("Failed to load API Harvester summary:", err);
    }
}
function renderApiHarvester(data) {

    if (!data) return;

    // ===== SpaceX =====
    if (data.SpaceX) {

        const rate = data.SpaceX.successRate ?? 0;
        document.getElementById("spxSuccessRate").textContent =
            (rate * 100).toFixed(2) + "%";

        const spxBody = document.getElementById("spxTableBody");
        spxBody.innerHTML = "";

        (data.SpaceX.launchesPerYear || []).forEach(l => {
            const row = `
                <tr>
                    <td>${l.year}</td>
                    <td>${l.count}</td>
                </tr>`;
            spxBody.innerHTML += row;
        });
    }

    // ===== CoinGecko =====
    if (data.CoinGecko) {

        document.getElementById("cgGainers").textContent =
            data.CoinGecko.change24hDistribution?.gainers ?? 0;

        document.getElementById("cgLosers").textContent =
            data.CoinGecko.change24hDistribution?.losers ?? 0;

        const cgBody = document.getElementById("cgTableBody");
        cgBody.innerHTML = "";

        (data.CoinGecko.topCoins || []).forEach(c => {
            const row = `
                <tr>
                    <td>${c.name}</td>
                    <td>${Number(c.value).toLocaleString()}</td>
                </tr>`;
            cgBody.innerHTML += row;
        });
    }

    // ===== OpenMeteo =====
    if (data.OpenMeteo) {

        const avg = data.OpenMeteo.avgTemp ?? 0;
        document.getElementById("omAvgTemp").textContent =
            avg.toFixed(2) + " °C";

        const omBody = document.getElementById("omTableBody");
        omBody.innerHTML = "";

        (data.OpenMeteo.hourlyTemp || []).forEach(h => {
            const row = `
                <tr>
                    <td>${h.time}</td>
                    <td>${h.value}</td>
                </tr>`;
            omBody.innerHTML += row;
        });
    }
}

const TELEMETRY_URL = "summary-report.json";



async function loadTelemetry() {
    try {
        const res = await fetch(TELEMETRY_URL + "?ts=" + Date.now());
        if (!res.ok) throw new Error("Telemetry file missing");

        const data = await res.json();
        renderTelemetry(data);
    } catch (err) {
        console.error("Telemetry load failed:", err);
    }
}

let tlmErrorChart = null;
let tlmSeverityChart = null;

function renderTelemetry(data) {

    // Cards
    document.getElementById("tlmTotalEvents").textContent =
        data.totalEvents.toLocaleString();

    document.getElementById("tlmProcessingTime").textContent =
        data.processingTimeMs.toFixed(2) + " ms";

    document.getElementById("tlmCritical").textContent =
        data.severityDistribution.Critical ?? 0;

    // Top Errors Chart
    const errorLabels = Object.keys(data.topErrors);
    const errorValues = Object.values(data.topErrors);

    if (tlmErrorChart) tlmErrorChart.destroy();

    tlmErrorChart = new Chart(
        document.getElementById("tlmErrorChart"),
        {
            type: "bar",
            data: {
                labels: errorLabels,
                datasets: [{
                    label: "Error Count",
                    data: errorValues
                }]
            }
        }
    );

    // Severity Chart
    const sevLabels = Object.keys(data.severityDistribution);
    const sevValues = Object.values(data.severityDistribution);

    if (tlmSeverityChart) tlmSeverityChart.destroy();

    tlmSeverityChart = new Chart(
        document.getElementById("tlmSeverityChart"),
        {
            type: "doughnut",
            data: {
                labels: sevLabels,
                datasets: [{
                    data: sevValues
                }]
            }
        }
    );

    // Device Table
    const tbody = document.getElementById("tlmDeviceTable");
    tbody.innerHTML = "";

    Object.values(data.deviceStatistics).forEach(d => {
        const row = `
            <tr>
                <td>${d.deviceId}</td>
                <td>${d.eventCount}</td>
                <td>${d.errorCount}</td>
                <td>${d.warningCount}</td>
                <td>${new Date(d.lastSeen).toLocaleString()}</td>
            </tr>
        `;
        tbody.innerHTML += row;
    });
}


loadTelemetry();
setInterval(loadTelemetry, REFRESH_MS);



loadApiHarvesterSummary();
setInterval(loadApiHarvesterSummary, REFRESH_MS);


loadMetrics();
setInterval(loadMetrics, REFRESH_MS);
