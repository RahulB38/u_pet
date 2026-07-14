(async function () {
    try {
        const res = await fetch('/Dashboard/ChartData');
        const data = await res.json();

        // Category doughnut chart
        const catLabels = data.categories.map(c => c.name);
        const catValues = data.categories.map(c => c.total);
        const catColors = data.categories.map(c => c.color);

        const catCtx = document.getElementById('categoryChart');
        if (catCtx) {
            new Chart(catCtx, {
                type: 'doughnut',
                data: {
                    labels: catLabels.length ? catLabels : ['No data'],
                    datasets: [{
                        data: catValues.length ? catValues : [1],
                        backgroundColor: catColors.length ? catColors : ['#e2e8f0'],
                        borderWidth: 0
                    }]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    plugins: {
                        legend: { position: 'bottom', labels: { padding: 16, usePointStyle: true } }
                    }
                }
            });
        }

        // Monthly trend bar chart
        const trendCtx = document.getElementById('trendChart');
        if (trendCtx) {
            new Chart(trendCtx, {
                type: 'bar',
                data: {
                    labels: data.trend.map(t => t.label),
                    datasets: [{
                        label: 'Spending (₹)',
                        data: data.trend.map(t => t.total),
                        backgroundColor: '#4f46e5',
                        borderRadius: 6
                    }]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    plugins: { legend: { display: false } },
                    scales: {
                        y: {
                            beginAtZero: true,
                            ticks: { callback: v => '₹' + v.toLocaleString('en-IN') }
                        }
                    }
                }
            });
        }
    } catch (e) {
        console.error('Failed to load chart data', e);
    }
})();
