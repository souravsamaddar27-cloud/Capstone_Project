// Medtronic Telemetry Dashboard - Static Version
// Professional telemetry analysis visualization

let reportData = null;

// Chart instances
let errorChart = null;
let severityChart = null;
let deviceChart = null;

// Initialize when page loads
window.addEventListener('load', function() {
    console.log('✅ Dashboard initialized');
    setupEventListeners();
    loadData(); // Auto-load on page open
});

function setupEventListeners() {
    // Reload button
    document.getElementById('loadDataBtn').onclick = function() {
        console.log('🔄 Reload button clicked');
        loadData();
    };
    
    // Search functionality
    const searchInput = document.getElementById('deviceSearch');
    if (searchInput) {
        searchInput.oninput = function(e) {
            filterDeviceTable(e.target.value);
        };
    }
}

// Main data loading function
async function loadData() {
    try {
        console.log('📊 Loading telemetry data...');
        updateStatus('loading', 'Loading data...');
        
        // Fetch JSON report
        const response = await fetch('summary-report.json?cacheBust=' + Date.now());
        
        if (!response.ok) {
            throw new Error('Report file not found! Please run the C# application first.');
        }
        
        reportData = await response.json();
        
        console.log('✅ Data loaded:', reportData.totalEvents, 'events');
        
        // Display everything
        displayStatistics();
        createCharts();
        populateDeviceTable();
        
        // Show sections
        document.getElementById('statsSection').classList.remove('hidden');
        document.getElementById('chartsSection').classList.remove('hidden');
        
        // Update timestamps
        document.getElementById('generatedDate').textContent = new Date(reportData.generatedAt).toLocaleString();
        document.getElementById('lastUpdateTime').textContent = new Date().toLocaleTimeString();
        
        updateStatus('active', 'Data Loaded');
        showToast('✅ Telemetry data loaded successfully!');
        
    } catch (error) {
        updateStatus('error', 'Load Failed');
        showToast('❌ Error: ' + error.message, 'error');
        console.error('❌ Load error:', error);
    }
}

// Update status badge
function updateStatus(status, text) {
    const badge = document.getElementById('statusBadge');
    const statusText = document.getElementById('statusText');
    
    if (!badge || !statusText) return;
    
    badge.className = 'status-badge';
    
    if (status === 'active') {
        badge.classList.add('status-active');
    } else if (status === 'loading') {
        badge.classList.add('status-loading');
    } else if (status === 'error') {
        badge.classList.add('status-error');
    }
    
    statusText.textContent = text;
}

// Display statistics with smooth animations
function displayStatistics() {
    animateCounter('totalEvents', reportData.totalEvents, 1200);
    animateCounter('criticalCount', reportData.severityDistribution['Critical'] || 0, 1200);
    animateCounter('deviceCount', Object.keys(reportData.deviceStatistics).length, 1200);
    document.getElementById('processingTime').textContent = reportData.processingTimeMs.toFixed(2) + 'ms';
}

// Animate counter with easing
function animateCounter(elementId, targetValue, duration) {
    const element = document.getElementById(elementId);
    if (!element) return;
    
    const startValue = 0;
    const range = targetValue - startValue;
    const startTime = performance.now();
    
    function update(currentTime) {
        const elapsed = currentTime - startTime;
        const progress = Math.min(elapsed / duration, 1);
        
        // Ease out cubic for smooth animation
        const easeProgress = 1 - Math.pow(1 - progress, 3);
        const currentValue = Math.round(startValue + (range * easeProgress));
        
        element.textContent = currentValue.toLocaleString();
        
        if (progress < 1) {
            requestAnimationFrame(update);
        } else {
            element.textContent = targetValue.toLocaleString();
        }
    }
    
    requestAnimationFrame(update);
}

// Create all charts
function createCharts() {
    createErrorChart();
    createSeverityChart();
    createDeviceChart();
}

// Top Errors Bar Chart
function createErrorChart() {
    const ctx = document.getElementById('errorChart');
    if (!ctx) return;
    
    // Destroy existing chart
    if (errorChart) errorChart.destroy();
    
    const labels = Object.keys(reportData.topErrors).slice(0, 8);
    const data = Object.values(reportData.topErrors).slice(0, 8);
    
    const totalErrors = data.reduce((a, b) => a + b, 0);
    document.getElementById('errorBadge').textContent = `${totalErrors.toLocaleString()} errors`;
    
    errorChart = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: labels,
            datasets: [{
                label: 'Error Count',
                data: data,
                backgroundColor: 'rgba(239, 68, 68, 0.8)',
                borderColor: 'rgba(239, 68, 68, 1)',
                borderWidth: 2,
                borderRadius: 8
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: true,
            animation: {
                duration: 1500,
                easing: 'easeInOutQuart'
            },
            plugins: {
                legend: { display: false },
                tooltip: {
                    backgroundColor: 'rgba(15, 23, 42, 0.95)',
                    padding: 12,
                    titleColor: '#f1f5f9',
                    bodyColor: '#f1f5f9',
                    borderColor: '#334155',
                    borderWidth: 1
                }
            },
            scales: {
                y: {
                    beginAtZero: true,
                    ticks: { color: '#94a3b8' },
                    grid: { color: 'rgba(51, 65, 85, 0.3)' }
                },
                x: {
                    ticks: { 
                        color: '#94a3b8',
                        maxRotation: 45,
                        minRotation: 45
                    },
                    grid: { display: false }
                }
            }
        }
    });
}

// Severity Distribution Doughnut Chart
function createSeverityChart() {
    const ctx = document.getElementById('severityChart');
    if (!ctx) return;
    
    if (severityChart) severityChart.destroy();
    
    const labels = Object.keys(reportData.severityDistribution);
    const data = Object.values(reportData.severityDistribution);
    
    const totalEvents = data.reduce((a, b) => a + b, 0);
    document.getElementById('severityBadge').textContent = `${totalEvents.toLocaleString()} events`;
    
    const colors = {
        'Low': 'rgba(16, 185, 129, 0.8)',
        'Medium': 'rgba(59, 130, 246, 0.8)',
        'High': 'rgba(245, 158, 11, 0.8)',
        'Critical': 'rgba(239, 68, 68, 0.8)'
    };
    
    const backgroundColors = labels.map(label => colors[label] || 'rgba(148, 163, 184, 0.8)');
    
    severityChart = new Chart(ctx, {
        type: 'doughnut',
        data: {
            labels: labels,
            datasets: [{
                data: data,
                backgroundColor: backgroundColors,
                borderColor: '#1e293b',
                borderWidth: 3
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: true,
            animation: {
                duration: 1500,
                easing: 'easeInOutQuart'
            },
            plugins: {
                legend: {
                    position: 'bottom',
                    labels: {
                        color: '#f1f5f9',
                        padding: 15,
                        font: { size: 12 }
                    }
                },
                tooltip: {
                    backgroundColor: 'rgba(15, 23, 42, 0.95)',
                    padding: 12,
                    titleColor: '#f1f5f9',
                    bodyColor: '#f1f5f9',
                    borderColor: '#334155',
                    borderWidth: 1
                }
            }
        }
    });
}

// Device Statistics Bar Chart
function createDeviceChart() {
    const ctx = document.getElementById('deviceChart');
    if (!ctx) return;
    
    if (deviceChart) deviceChart.destroy();
    
    const devices = Object.values(reportData.deviceStatistics);
    const labels = devices.map(d => d.deviceId);
    const eventCounts = devices.map(d => d.eventCount);
    const errorCounts = devices.map(d => d.errorCount);
    const warningCounts = devices.map(d => d.warningCount);
    
    deviceChart = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: labels,
            datasets: [
                {
                    label: 'Total Events',
                    data: eventCounts,
                    backgroundColor: 'rgba(59, 130, 246, 0.8)',
                    borderColor: 'rgba(59, 130, 246, 1)',
                    borderWidth: 2,
                    borderRadius: 6
                },
                {
                    label: 'Errors',
                    data: errorCounts,
                    backgroundColor: 'rgba(239, 68, 68, 0.8)',
                    borderColor: 'rgba(239, 68, 68, 1)',
                    borderWidth: 2,
                    borderRadius: 6
                },
                {
                    label: 'Warnings',
                    data: warningCounts,
                    backgroundColor: 'rgba(245, 158, 11, 0.8)',
                    borderColor: 'rgba(245, 158, 11, 1)',
                    borderWidth: 2,
                    borderRadius: 6
                }
            ]
        },
        options: {
            responsive: true,
            maintainAspectRatio: true,
            animation: {
                duration: 1500,
                easing: 'easeInOutQuart'
            },
            plugins: {
                legend: { display: false },
                tooltip: {
                    backgroundColor: 'rgba(15, 23, 42, 0.95)',
                    padding: 12,
                    titleColor: '#f1f5f9',
                    bodyColor: '#f1f5f9',
                    borderColor: '#334155',
                    borderWidth: 1
                }
            },
            scales: {
                y: {
                    beginAtZero: true,
                    ticks: { color: '#94a3b8' },
                    grid: { color: 'rgba(51, 65, 85, 0.3)' }
                },
                x: {
                    ticks: { color: '#94a3b8' },
                    grid: { display: false }
                }
            }
        }
    });
}

// Populate device details table
function populateDeviceTable() {
    const tbody = document.getElementById('deviceTableBody');
    if (!tbody) return;
    
    tbody.innerHTML = '';
    
    const devices = Object.values(reportData.deviceStatistics);
    
    devices.forEach((device, index) => {
        const row = document.createElement('tr');
        row.style.animationDelay = `${index * 0.05}s`;
        row.classList.add('table-row-appear');
        
        let statusClass = 'status-active';
        let statusText = 'Normal';
        
        if (device.errorCount > 100) {
            statusClass = 'status-error';
            statusText = 'Critical';
        } else if (device.errorCount > 50) {
            statusClass = 'status-warning';
            statusText = 'Warning';
        }
        
        row.innerHTML = `
            <td><strong>${device.deviceId}</strong></td>
            <td>${device.eventCount.toLocaleString()}</td>
            <td><span class="error-badge">${device.errorCount.toLocaleString()}</span></td>
            <td><span class="warning-badge">${device.warningCount.toLocaleString()}</span></td>
            <td>${new Date(device.lastSeen).toLocaleString()}</td>
            <td><span class="status-badge-table ${statusClass}">${statusText}</span></td>
        `;
        
        tbody.appendChild(row);
    });
}

// Filter device table based on search
function filterDeviceTable(searchTerm) {
    const tbody = document.getElementById('deviceTableBody');
    if (!tbody) return;
    
    const rows = tbody.getElementsByTagName('tr');
    
    for (let row of rows) {
        const deviceId = row.cells[0].textContent.toLowerCase();
        if (deviceId.includes(searchTerm.toLowerCase())) {
            row.style.display = '';
        } else {
            row.style.display = 'none';
        }
    }
}

// Show toast notification
function showToast(message, type = 'success') {
    const toast = document.getElementById('toast');
    const toastMessage = document.getElementById('toastMessage');
    
    if (!toast || !toastMessage) return;
    
    toastMessage.textContent = message;
    toast.className = 'toast';
    
    if (type === 'error') {
        toast.classList.add('toast-error');
    }
    
    toast.classList.add('show');
    
    setTimeout(() => {
        toast.classList.remove('show');
    }, 3000);
}