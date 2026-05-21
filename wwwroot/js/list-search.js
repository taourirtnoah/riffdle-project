(function () {
    function debounce(fn, delay) {
        let t = null;
        return function (...args) {
            clearTimeout(t);
            t = setTimeout(() => fn.apply(this, args), delay);
        };
    }

    async function fetchIds(url, query) {
        try {
            const res = await fetch(`${url}?q=${encodeURIComponent(query)}`, {
                headers: { Accept: 'application/json' }
            });
            if (!res.ok) return null;
            const data = await res.json();
            if (!Array.isArray(data)) return null;
            return new Set(data.map(item => String(item.id)));
        } catch (e) {
            console.error('List search request failed', e);
            return null;
        }
    }

    document.addEventListener('DOMContentLoaded', () => {
        document.querySelectorAll('[data-list-search-input]').forEach((input) => {
            const url = input.dataset.listSearchUrl || '';
            const targetSelector = input.dataset.listTarget || '';
            const target = targetSelector ? document.querySelector(targetSelector) : null;
            if (!target) return;

            const groups = () => {
                const rows = Array.from(target.querySelectorAll('tr[data-item-id]'));
                const grouped = new Map();

                rows.forEach((row) => {
                    const itemId = row.dataset.itemId;
                    if (!grouped.has(itemId)) {
                        grouped.set(itemId, []);
                    }

                    grouped.get(itemId).push(row);
                });

                return Array.from(grouped.values());
            };

            const groupText = (rows) => {
                const explicit = rows
                    .map((row) => row.dataset.searchText || '')
                    .join(' ')
                    .trim();

                if (explicit) {
                    return explicit.toLowerCase();
                }

                return rows
                    .map((row) => row.textContent || '')
                    .join(' ')
                    .trim()
                    .toLowerCase();
            };

            const showGroup = (rows, visible) => {
                rows.forEach((row) => {
                    row.style.display = visible ? '' : 'none';
                });
            };

            const localFilter = (query) => {
                const q = query.trim().toLowerCase();

                groups().forEach((rows) => {
                    showGroup(rows, q === '' || groupText(rows).includes(q));
                });
            };

            const doSearch = async (query) => {
                if (url) {
                    const ids = await fetchIds(url, query);
                    if (ids === null) {
                        localFilter(query);
                        return;
                    }

                    groups().forEach((rows) => {
                        const id = rows[0]?.dataset.itemId;
                        showGroup(rows, query.trim() === '' || ids.has(String(id)));
                    });
                } else {
                    localFilter(query);
                }
            };

            input.addEventListener('input', debounce((e) => {
                doSearch(e.target.value);
            }, 250));
        });
    });
})();
