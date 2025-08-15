
let MIN_TIMESTAMP = null;
let MAX_TIMESTAMP = null;

document.getElementById("timeRange").addEventListener("change", () => {
    const symbol = document.getElementById("symbolSelect").value;
    loadChartData(symbol);
});

document.getElementById("chartType").addEventListener("change", () => {
    const symbol = document.getElementById("symbolSelect").value;
    loadChartData(symbol);
});

async function loadSymbols() {
    const res = await fetch("/symbols");
    const symbols = await res.json();
    const symbolSelect = document.getElementById("symbolSelect");

    symbolSelect.innerHTML = symbols
        .map((s, i) => `<option value="${s}" ${i === 1 ? "selected" : ""}>${s}</option>`)
        .join("");

    const defaultSymbol = symbols[1];

    await initMeta(defaultSymbol);

    symbolSelect.addEventListener("change", async (e) => {
        await initMeta(e.target.value);
    });
}


async function initMeta(symbol) {
    const res = await fetch(`/history/meta?symbol=${encodeURIComponent(symbol)}`);
    const meta = await res.json();
    MIN_TIMESTAMP = meta.min;
    MAX_TIMESTAMP = meta.max;

    loadChartData(symbol);
}

async function loadChartData(symbol) {
    const container = document.getElementById('chartContainer');
    container.innerHTML = "";
    let chart = LightweightCharts.createChart(container, {
        width: 1200,
        height: 600
    });
    let series = null;
    const range = document.getElementById("timeRange").value;
    const chartType = document.getElementById("chartType").value;

    const to = MAX_TIMESTAMP;
    let from;

    switch (range) {
        case "1d": from = to - 86400; break;
        case "1w": from = to - 604800; break;
        case "1m": from = to - 2629800; break;
        case "1y": from = to - 31557600; break;
        case "2y": from = to - 63115200; break;
        default: from = MIN_TIMESTAMP;
    }
    if (from < MIN_TIMESTAMP) from = MIN_TIMESTAMP;

    const url = `/history?symbol=${symbol}&from=${from}&to=${to}`;
    try {
        const res = await fetch(url);
        const data = await res.json();

        const candleData = data.t.map((timestamp, index) => ({
            time: timestamp,
            open: data.o[index],
            high: data.h[index],
            low: data.l[index],
            close: data.c[index]
        }));

        const areaData = data.t.map((timestamp, index) => ({
            time: timestamp,
            value: data.v[index]
        }));

        if (series) {
            chart.removeSeries(series);
        }

        if (chartType === "candlestick") {
            series = chart.addSeries(LightweightCharts.CandlestickSeries, {
                upColor: '#26a69a',
                downColor: '#ef5350',
                borderVisible: false,
                wickUpColor: '#26a69a',
                wickDownColor: '#ef5350',
            });
            series.setData(candleData);
        } else if (chartType === "area") {
            series = chart.addSeries(LightweightCharts.AreaSeries, {
                lineColor: '#2962FF', topColor: '#2962FF',
                bottomColor: 'rgba(41, 98, 255, 0.28)',
            });
            series.setData(areaData);
        }

        chart.timeScale().fitContent();
    }
    catch (err) {
        alert("Error: " + err);
    }
}

loadSymbols()