(async () => {
    let currentPage = 1;
    const logsTableBody = document.getElementById('logsTable').querySelector('tbody');
    const selectPerPage = document.getElementById('selectPerPage');

    async function fetchLogs(token, page, n) {
        const spinner = document.getElementById('spinner');
        spinner.style.display = 'block';
        const response = await fetch(`../api/Logs/Get?page=${page}&n=${n}`, {
            headers: {
                'Authorization': `Bearer ${token}`
            }, method: 'GET'
        });
        if (!response.ok) {
            switch (response.status) {
                case 401:
                    alert('Unauthorized');
                    window.location.href = 'login.html';
                    break;
                case 404:
                    alert('Not found');
                    break;
                default:
                    alert('Something went wrong');
                    break;
            }
        }
        const logs = await response.json();
        displayLogs(logs);
        spinner.style.display = 'none';

    }

    function displayLogs(logs) {
        logsTableBody.innerHTML = '';
        logs.items.forEach(log => {
            const row = document.createElement('tr');
            switch (log.lvl) {
                case 1:
                    row.classList.add('varn-low');
                    break;
                case 2:
                    row.classList.add('varn-medium');
                    break;
                case 3:
                    row.classList.add('varn-high');
                    break;
                default:
                    break;
            }
            row.innerHTML = `
                <td>${log.date}</td>
                <td>${log.message}</td>
            `;
            logsTableBody.appendChild(row);
        });
        displayPageNum(logs);
    }

    function displayPageNum(logs) {
        const ul = document.getElementById('pag');
        ul.innerHTML = '';

        const first = document.createElement('li');
        first.classList.add('page-item');
        first.innerHTML = `<a class="page-link" href="#">First</a>`;
        first.addEventListener('click', async () => {
            currentPage = 1;
            await fetchLogs(localStorage.getItem('jwt'), currentPage, selectPerPage.value);
        });
        ul.appendChild(first);

        const prev = document.createElement('li');
        prev.classList.add('page-item');
        prev.innerHTML = `<a class="page-link" href="#">Previous</a>`;
        prev.addEventListener('click', async () => {
            if (currentPage > 1) {
                currentPage--;
                await fetchLogs(localStorage.getItem('jwt'), currentPage, selectPerPage.value);
            }
        });
        ul.appendChild(prev);


        for (let i = logs.fromPage; i <= logs.toPage; i++) {
            const li = document.createElement('li');
            li.classList.add('page-item');
            if (i === currentPage) li.classList.add('active');
            li.innerHTML = `<a class="page-link" href="#">${i}</a>`;
            li.addEventListener('click', async () => {
                currentPage = i;
                await fetchLogs(localStorage.getItem('jwt'), currentPage, selectPerPage.value);
            });
            ul.appendChild(li);
        }

        const next = document.createElement('li');
        next.classList.add('page-item');
        next.innerHTML = `<a class="page-link" href="#">Next</a>`;
        next.addEventListener('click', async () => {
            if (currentPage < logs.lastPage) {
                currentPage++;
                await fetchLogs(localStorage.getItem('jwt'), currentPage, selectPerPage.value);
            }
        });
        ul.appendChild(next);

        const last = document.createElement('li');
        last.classList.add('page-item');
        last.innerHTML = `<a class="page-link" href="#">Last</a>`;
        last.addEventListener('click', async () => {
            currentPage = logs.lastPage;
            await fetchLogs(localStorage.getItem('jwt'), currentPage, selectPerPage.value);
        });
        ul.appendChild(last);
    }

    selectPerPage.addEventListener('change', async () => {
        currentPage = 1;
        await fetchLogs(localStorage.getItem('jwt'), currentPage, selectPerPage.value);
    });

    const btn = document.getElementById('prev');
    btn.addEventListener('click', async () => {
        await fetchLogs(localStorage.getItem('jwt'), currentPage, selectPerPage.value);
    });
})();