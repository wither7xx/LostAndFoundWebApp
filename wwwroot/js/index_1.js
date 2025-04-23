document.addEventListener('DOMContentLoaded', () => {
    const itemList = document.getElementById('itemList');
    const searchForm = document.getElementById('searchForm');

    // 模拟的失物招领数据
    const items = [
        { id: 1, name: '钱包', location: '图书馆', status: 'lost', date: '2025-04-18', campus: 'campus1', validity: 'valid', category: 'documents', contact: '1234567890' },
        { id: 2, name: '手机', location: '食堂', status: 'found', date: '2025-04-17', campus: 'campus2', validity: 'valid', category: 'electronics', contact: '0987654321' },
        // 更多数据...
    ];

    // 动态填充物品列表
    function displayItems(filteredItems = items) {
        itemList.innerHTML = '';
        filteredItems.forEach(item => {
            const li = document.createElement('li');
            li.textContent = `${item.name} - ${item.status} - ${item.date} - ${item.campus}`;
            itemList.appendChild(li);
        });
    }

    displayItems();

    // 搜索功能
    searchForm.addEventListener('submit', (e) => {
        e.preventDefault();
        const name = document.getElementById('searchName').value.toLowerCase();
        const status = document.getElementById('searchStatus').value;
        const startDate = document.getElementById('searchStartDate').value;
        const endDate = document.getElementById('searchEndDate').value;
        const campus = document.getElementById('searchCampus').value;
        const validity = document.getElementById('searchValidity').value;
        const category = document.getElementById('searchCategory').value;

        const filteredItems = items.filter(item => {
            return (
                (!name || item.name.toLowerCase().includes(name)) &&
                (!status || item.status === status) &&
                (!startDate || item.date >= startDate) &&
                (!endDate || item.date <= endDate) &&
                (!campus || item.campus === campus) &&
                (!validity || item.validity === validity) &&
                (!category || item.category === category)
            );
        });

        displayItems(filteredItems);
    });
});