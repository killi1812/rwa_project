(async () => {
    let currentPage = 1;
    const logsTableBody = document.getElementById('logsTable').querySelector('tbody');
    const selectPerPage = document.getElementById('selectPerPage');
    const prevPageButton = document.getElementById('prevPage');
    const nextPageButton = document.getElementById('nextPage');

    async function fetchLogs(token, page, n) {
        const response = await fetch(`../api/Logs/GetLogs?page=${page}&n=${n}`, {
            headers: {
                'Authorization': `Bearer ${token}`
            },
            method: 'GET'
        });
        const logs = await response.json();
        displayLogs(logs);
    }

    function displayLogs(logs) {
        const spinner = document.getElementById('spinner');
        spinner.style.display = 'none';

        logsTableBody.innerHTML = '';
        logs.forEach(log => {
            const row = document.createElement('tr');
            row.innerHTML = `
                <td>${log.date}</td>
                <td>${log.message}</td>
            `;
            logsTableBody.appendChild(row);
        });
    }

    function displayPageNum() {
        const pageNum = document.getElementById('page');
        pageNum.innerHTML = currentPage;
    }

    selectPerPage.addEventListener('change', async () => {
        currentPage = 1;
        displayPageNum();
        await fetchLogs(localStorage.getItem('jwt'), currentPage, selectPerPage.value);
    });

    prevPageButton.addEventListener('click', async () => {
        if (currentPage > 1) {
            currentPage--;
            displayPageNum();
            await fetchLogs(localStorage.getItem('jwt'), currentPage, selectPerPage.value);
        }
    });

    nextPageButton.addEventListener('click', async () => {
        currentPage++;
        displayPageNum();
        await fetchLogs(localStorage.getItem('jwt'), currentPage, selectPerPage.value);
    });

    await fetchLogs(localStorage.getItem('jwt'), currentPage, selectPerPage.value);
})();