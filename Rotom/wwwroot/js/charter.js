function getLiveChartConfig() {
    const data = {
        datasets: [{
            data: [],
            fill: {
                target: 'origin',
                below: 'rgba(231, 76, 60, 0.7)',
                above: 'rgba(0, 188, 140, 0.7)'
            },
            tension: 0.1,
            pointRadius: 0,
            pointHitRadius: 0,
            showLine: false,
        }, {
            data: [],
            fill: false,
            borderColor: 'rgb(231, 76, 60)',
            pointRadius: 0,
            pointHitRadius: 0
        }, {
            data: [],
            fill: false,
            borderColor: 'rgb(0, 188, 140)',
            pointRadius: 0,
            pointHitRadius: 0
        }]
    };
    return {
        type: 'line',
        data: data,
        options: {
            scales: {
                x: {
                    type: 'time',
                    time: {
                        unit: 'second',
                        displayFormats: {
                            second: 'kk:mm:ss'
                        }
                    },
                    ticks: {
                        callback: function (val, index) {
                            if (index % 30 == 0) {
                                return val;
                            }
                            return null;
                        }
                    }
                },
                y: {
                    title: {
                        display: true,
                        text: 'W'
                    },
                    beginAtZero: true,
                    suggestedMin: -200,
                    suggestedMax: 100,
                    ticks: {
                        callback: function (val, index) {
                            if (!isNaN(val)) {
                                return Math.abs(val);
                            }
                            return val;
                        }
                    }
                }
            },
            plugins: {
                legend: {
                    display: false
                }
            }
        }
    };
}

function getHistoryChartConfig(graphType) {
    const data = {
        datasets: [{
            data: [],
            fill: {
                target: 'origin',
                below: 'rgba(231, 76, 60, 0.7)',
                above: 'rgba(0, 188, 140, 0.7)'
            },
            tension: 0.1,
            pointRadius: 0,
            pointHitRadius: 0,
            showLine: false,
        }]
    };
    return {
        type: 'line',
        data: data,
        options: {
            scales: {
                x: {
                    type: 'time',
                    time: {
                        unit: graphType == 'Daily' ? 'hour' : 'day',
                        displayFormats: {
                            hour: 'kk:mm',
                            day: 'd-M'
                        }
                    }
                },
                y: {
                    title: {
                        display: true,
                        text: 'kWh'
                    },
                    beginAtZero: true,
                    suggestedMin: -0.1,
                    suggestedMax: 0.1,
                    ticks: {
                        callback: function (val, index) {
                            if (!isNaN(val)) {
                                return Math.abs(val);
                            }
                            return val;
                        },
                        precision: 3
                    }
                }
            },
            plugins: {
                legend: {
                    display: false
                }
            }
        }
    };
}
