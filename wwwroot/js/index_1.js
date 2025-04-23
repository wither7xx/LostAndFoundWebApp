document.addEventListener('DOMContentLoaded', () => {
    const itemList = document.getElementById('itemList');
    const searchForm = document.getElementById('searchForm');



    // 从后端获取物品数据
    async function fetchItems() {
        try {
            const response = await fetch('/Index?handler=Items'); // 调用 OnGetItems 方法
            if (!response.ok) {
                throw new Error('Failed to fetch items');
            }
            const items = await response.json();
            displayItems(items);
        } catch (error) {
            console.error('Error fetching items:', error);
        }
    }

    // 动态填充物品列表
    function displayItems(filteredItems = mockItems) {
        itemList.innerHTML = '';
        filteredItems.forEach(item => {
            const li = document.createElement('li');
            li.textContent = `${item.name} - ${item.status} - ${item.date} - ${item.campus}`;
            itemList.appendChild(li);
        });
    }

    // 初始化页面时加载物品数据
    fetchItems();

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

        fetchItems().then(items => {
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
        })
    });
});