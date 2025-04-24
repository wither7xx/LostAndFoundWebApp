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
        const itemList = document.getElementById('itemList');
        itemList.innerHTML = '';

        filteredItems.forEach(item => {
            const row = document.createElement('tr');
            const campusMap1 = {
                campus1: '前卫南区',
                campus2: '南岭校区',
                campus3: '和平校区',
                campus4: '朝阳校区',
                campus5: '新民校区',
                campus6: '南湖校区'
            };
            const campusMap2 = {
                electronics: '电子设备',
                documents: '文档/书籍',
                dailyitems: '生活用品',
                academic: '学术用品',
                clothing: '衣物',
                valuables: '贵重物品',
                sports: '运动用品',
                others:'其它物品'
            };

            // 在模板字符串中为操作列预留空<td>
            row.innerHTML = `
            <td>${item.itemId}-${item.name}</td>
            <td>${item.status}</td>
            <td>${campusMap2[item.category]}</td>
            <td>${item.time}</td>
            <td>${item.location}</td>
            <td>${campusMap1[item.campus]}</td>
            <td>${item.isValid ? '是' : '否'}</td>
            <td></td>
        `;

            // 找到最后一个td（操作列）
            const actionTd = row.querySelector('td:last-child');

            // 创建并添加按钮
            const button = document.createElement('button');
            button.textContent = '查看';
            button.className = 'detail-btn'; // 应用 CSS 类
            button.addEventListener('click', () => handleAction(item.itemId));
            actionTd.appendChild(button);

            itemList.appendChild(row);
        });
    }

    // 全局挂载操作函数
    window.handleAction = function (itemId) {
        console.log('操作项目ID:', itemId);
    };

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