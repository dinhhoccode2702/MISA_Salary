import { ref, computed, watch, isRef } from 'vue';

/**
 * Composable xử lý logic phân trang
 * @param {Ref<Array> | Array} listData - Mảng dữ liệu reactivity cần phân trang
 * @param {Number} defaultPageSize - Kích thước mặc định của một trang (mặc định: 50)
 */

export default function usePagination(listData, defaultPageSize = 50) {
    // Đảm bảo listData luôn là một Ref để có tính reactivity
    const dataList = isRef(listData) ? listData : computed(() => listData);
    
    const currentPage = ref(1);
    const pageSize = ref(defaultPageSize);
    
    // Tính toán số lượng tổng
    const totalItems = computed(() => dataList.value.length);
    
    // Tính index bắt đầu và index kết thúc
    const startIndex = computed(() => (currentPage.value - 1) * pageSize.value);
    const endIndex = computed(() => Math.min(startIndex.value + pageSize.value, totalItems.value));
    
    // Lấy ra danh sách cho trang hiện tại
    const paginatedData = computed(() => {
        return dataList.value.slice(startIndex.value, endIndex.value);
    });

    const canPrev = computed(() => currentPage.value > 1);
    const canNext = computed(() => endIndex.value < totalItems.value);

    // Xử lý chuyển trang
    const prevPage = () => { if (canPrev.value) currentPage.value-- };
    const nextPage = () => { if (canNext.value) currentPage.value++ };

    // Reset lại số trang về 1 khi đổi pageSize
    watch(pageSize, () => {
        currentPage.value = 1;
    });

    // Giới hạn lại số trang nếu bị lố trang khi xóa dữ liệu
    watch(totalItems, () => {
        const maxPage = Math.ceil(totalItems.value / pageSize.value) || 1;
        if (currentPage.value > maxPage) {
            currentPage.value = maxPage;
        }
    });

    return {
        currentPage,
        pageSize,
        totalItems,
        startIndex,
        endIndex,
        paginatedData,
        canPrev,
        canNext,
        prevPage,
        nextPage
    };
}
