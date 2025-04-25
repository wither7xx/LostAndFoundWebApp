document.addEventListener('DOMContentLoaded', () => {
    const itemList = document.getElementById('itemList');
    const searchForm = document.getElementById('searchForm');
    let currentPage = 1;
    let totalPages = 1;

    // 从后端获取物品数据
    async function fetchItems() {
        try {
            const response = await fetch(`/Index?handler=Items&page=${currentPage}&pageSize=${document.getElementById('pageSize').value}`);
            if (!response.ok) {
                throw new Error('Failed to fetch items');
            }
            const data = await response.json();
            // 检查返回的数据结构
            console.log('Fetched data:', data);
            // 使用 data.items 而不是直接使用 data
            displayItems(data.items || []);
            updatePagination(data.totalItems, data.currentPage, data.totalPages);
        } catch (error) {
            console.error('Error fetching items:', error);
        }
    }

    // 动态填充物品列表
    function displayItems(filteredItems) {
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
                others: '其它物品'
            };

            row.innerHTML = `
                <td>${item.itemId}-${item.name}</td>
                <td>${item.status === 'Lost' ? '丢失物品' : '拾获物品'}</td>
                <td>${campusMap2[item.category] || item.category}</td>
                <td>${formatDate(item.time)}</td>
                <td>${item.location}</td>
                <td>${campusMap1[item.campus] || item.campus}</td>
                <td>${item.isValid ? '是' : '否'}</td>
                <td><button class = "detail-btn" onclick="handleAction(${item.itemId})">查看</button></td>
            `;
            itemList.appendChild(row);
        });
    }

    // 更新分页控件
    function updatePagination(totalItems, currentPg, totalPgs) {
        console.log('Updating pagination:', { totalItems, currentPg, totalPgs }); // 添加调试日志
        const pageNumbers = document.getElementById('pageNumbers');
        pageNumbers.innerHTML = '';
        totalPages = totalPgs;

        // 更新按钮状态
        document.getElementById('firstPage').disabled = currentPg === 1;
        document.getElementById('prevPage').disabled = currentPg === 1;
        document.getElementById('nextPage').disabled = currentPg === totalPgs;
        document.getElementById('lastPage').disabled = currentPg === totalPgs;

        // 生成页码按钮
        let startPage = Math.max(1, currentPg - 2);
        let endPage = Math.min(totalPgs, startPage + 4);

        if (endPage - startPage < 4) {
            startPage = Math.max(1, endPage - 4);
        }

        for (let i = startPage; i <= endPage; i++) {
            const button = document.createElement('button');
            button.textContent = i;
            if (i === currentPg) {
                button.classList.add('active');
            }
            button.addEventListener('click', () => goToPage(i));
            pageNumbers.appendChild(button);
        }

        // 更新页码信息
        document.getElementById('currentPage').textContent = currentPg;
        document.getElementById('totalPages').textContent = totalPgs;
    }

    // 格式化日期
    function formatDate(dateString) {
        if (!dateString) return '';
        const date = new Date(dateString);
        return date.toLocaleString('zh-CN', {
            year: 'numeric',
            month: '2-digit',
            day: '2-digit',
            hour: '2-digit',
            minute: '2-digit'
        });
    }

    // 页面跳转
    function goToPage(page) {
        console.log('Going to page:', page); // 添加调试日志
        currentPage = page;
        if (hasActiveSearch()) {
            searchForm.dispatchEvent(new Event('submit'));
        } else {
            fetchItems();
        }
    }

    // 检查是否有活动的搜索条件
    function hasActiveSearch() {
        return document.getElementById('searchName').value ||
            document.getElementById('searchStatus').value ||
            document.getElementById('searchStartDate').value ||
            document.getElementById('searchEndDate').value ||
            document.getElementById('searchCampus').value ||
            document.getElementById('searchValidity').value ||
            document.getElementById('searchCategory').value;
    }

    // 搜索表单提交处理
    searchForm.addEventListener('submit', async (e) => {
        e.preventDefault();
        const searchParams = {
            name: document.getElementById('searchName')?.value.trim() || null,
            status: document.getElementById('searchStatus')?.value || null,
            startDate: document.getElementById('searchStartDate')?.value
                ? new Date(document.getElementById('searchStartDate').value).toISOString()
                : null,
            endDate: document.getElementById('searchEndDate')?.value
                ? new Date(document.getElementById('searchEndDate').value).toISOString()
                : null,
            campus: document.getElementById('searchCampus')?.value || null,
            isValid: document.getElementById('searchValidity')?.value === 'valid'
                ? true
                : (document.getElementById('searchValidity')?.value === 'invalid' ? false : null),
            // 检查元素是否存在
            onlyMyItems: document.getElementById('onlyMyItems')?.checked || false,
            userID: null,
            category: document.getElementById('searchCategory')?.value || null,
            page: currentPage,
            pageSize: parseInt(document.getElementById('pageSize')?.value || '10')
        };

        console.log('Search parameters:', searchParams); // 添加调试日志

        try {
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

            const data = await response.json();
            console.log('Search response:', data); // 添加调试日志

            if (data && Array.isArray(data.items)) {
                displayItems(data.items);
                // 确保这些值都存在
                const totalItems = data.totalItems || 0;
                const currentPg = data.currentPage || 1;
                const totalPgs = data.totalPages || 1;
                updatePagination(totalItems, currentPg, totalPgs);
            } else {
                console.error('Invalid response format:', data);
                displayItems([]);
                updatePagination(0, 1, 1);
            }

        } catch (error) {
            console.error('搜索错误:', error);
            displayItems([]);
            updatePagination(0, 1, 1);
        }
    });

    // 分页按钮事件监听
    document.getElementById('firstPage').addEventListener('click', () => goToPage(1));
    document.getElementById('prevPage').addEventListener('click', () => goToPage(currentPage - 1));
    document.getElementById('nextPage').addEventListener('click', () => goToPage(currentPage + 1));
    document.getElementById('lastPage').addEventListener('click', () => goToPage(totalPages));

    // 每页显示数量变化监听
    document.getElementById('pageSize').addEventListener('change', () => {
        console.log('Page size changed:', document.getElementById('pageSize').value); // 添加调试日志
        currentPage = 1;
        if (hasActiveSearch()) {
            searchForm.dispatchEvent(new Event('submit'));
        } else {
            fetchItems();
        }
    });

    // 初始化页面时加载物品数据
    fetchItems();

    // 全局挂载操作函数
    window.handleAction = async function (itemId) {
        console.log('操作项目ID:', itemId);
        try {
            const response = await fetch(`/Index?handler=ItemDetails&itemId=${itemId}`);
            if (!response.ok) {
                throw new Error('获取物品详情失败');
            }

            const data = await response.json();
            if (data.success) {
                showItemDetails(data.item);
            } else {
                alert(data.message || '获取物品详情失败');
            }
        } catch (error) {
            console.error('Error:', error);
            alert('获取物品详情失败');
        }
    };

    // 添加显示物品详情的函数
    function showItemDetails(item) {
        const campusMap = {
            campus1: '前卫南区',
            campus2: '南岭校区',
            campus3: '和平校区',
            campus4: '朝阳校区',
            campus5: '新民校区',
            campus6: '南湖校区'
        };

        const categoryMap = {
            electronics: '电子设备',
            documents: '文档/书籍',
            dailyitems: '生活用品',
            academic: '学术用品',
            clothing: '衣物',
            valuables: '贵重物品',
            sports: '运动用品',
            others: '其它物品'
        };

        const statusMap = {
            'Lost': '丢失物品',
            'Found': '拾获物品'
        };

        // 使用已有的模态框元素
        const modal = document.getElementById('detailModal');
        const overlay = document.getElementById('modalOverlay');
        const modalContent = document.getElementById('modalContent');

        // 填充内容
        modalContent.innerHTML = `
        <div class="item-details">
            <p><strong>物品编号：</strong>${item.itemId}</p>
            <p><strong>物品名称：</strong>${item.name}</p>
            <p><strong>状态：</strong>${statusMap[item.status] || item.status}</p>
            <p><strong>分类：</strong>${categoryMap[item.category] || item.category}</p>
            <p><strong>时间：</strong>${formatDate(item.time)}</p>
            <p><strong>地点：</strong>${item.location}</p>
            <p><strong>校区：</strong>${campusMap[item.campus] || item.campus}</p>
            <p><strong>描述：</strong>${item.description || '无'}</p>
            <p><strong>联系方式：</strong>${item.contactInfo || '无'}</p>
            <p><strong>是否有效：</strong>${item.isValid ? '是' : '否'}</p>
        </div>
    `;

        // 显示模态框和遮罩层
        modal.style.display = 'block';
        overlay.style.display = 'block';

        // 关闭按钮事件
        const closeBtn = modal.querySelector('.close-btn');
        const closeModal = () => {
            modal.style.display = 'none';
            overlay.style.display = 'none';
        };

        closeBtn.onclick = closeModal;

        // 点击遮罩层关闭
        overlay.onclick = closeModal;

        // ESC键关闭
        const escHandler = (event) => {
            if (event.key === 'Escape') {
                closeModal();
                document.removeEventListener('keydown', escHandler);
            }
        };
        document.addEventListener('keydown', escHandler);
    };
});