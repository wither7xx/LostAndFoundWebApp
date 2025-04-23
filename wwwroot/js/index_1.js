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
            const campusMap = {
                campus1: '前卫南区',
                campus2: '南岭校区',
                campus3: '和平校区',
                campus4: '朝阳校区',
                campus5: '新民校区',
                campus6: '南湖校区'
            };

            li.textContent = `${item.name} - ${item.status} -${item.location} - ${item.description} - ${item.contactInfo} - ${item.category}-  ${item.time} - ${campusMap[item.campus]}`;
            itemList.appendChild(li);
        });
    }

    // 初始化页面时加载物品数据
    fetchItems();

    // 搜索功能
    searchForm.addEventListener('submit', async (e) => {
        e.preventDefault();
        const searchParams = {
            name: document.getElementById('searchName').value.trim() || null,
            status: document.getElementById('searchStatus').value || null,
            startDate: document.getElementById('searchStartDate').value
                ? new Date(document.getElementById('searchStartDate').value).toISOString()
                : null,
            endDate: document.getElementById('searchEndDate').value
                ? new Date(document.getElementById('searchEndDate').value).toISOString()
                : null,
            campus: document.getElementById('searchCampus').value || null,
            isValid: document.getElementById('searchValidity').value === 'valid'
                ? 1
                : (document.getElementById('searchValidity').value === 'invalid' ? 0 : null),
            category: document.getElementById('searchCategory').value || null
        };
        console.log('最终参数:', JSON.stringify(searchParams, null, 2));
        try {
            // 调用后端API
            const response = await fetch('/Index?handler=Search', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(searchParams)
            });

            if (!response.ok) {
                throw new Error(`请求失败: ${response.status}`);
            }

            const items = await response.json();
            displayItems(items);

        } catch (error) {
            console.error('搜索错误:', error);
            showErrorToast('搜索失败，请稍后重试');  // 显示错误提示
        }
    });
});