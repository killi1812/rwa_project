(async () => {
    const logsTableBody = document.getElementById('logsTable').querySelector('tbody');

    async function fetchLogs(token) {
        const response = await fetch('api/logs', {
            headers: {
                'Authorization': `Bearer ${token}`
            }
        });
        const logs = await response.json();
        displayLogs(logs);
    }

    function displayLogs(logs) {
        logsTableBody.innerHTML = '';
        logs.forEach(log => {
            const row = document.createElement('tr');
            row.innerHTML = `
                <td>${log.timestamp}</td>
                <td>${log.level}</td>
                <td>${log.message}</td>
            `;
            logsTableBody.appendChild(row);
        });
    }

    await fetchLogs(localStorage.getItem('token'));
})();