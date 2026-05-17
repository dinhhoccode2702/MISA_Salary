import { ref, computed, watch } from 'vue';
import { getCandidateFromStorage, clearCandidateStorage , saveDataToStorage, deleteCandidateFromStorage} from '../services/storage';

const originalCandidates = ref([]);
const isLoading = ref(false);
const error = ref(null);
const searchKeyword = ref(''); // Biến lưu từ khóa tìm kiếm


export default function useCandidate() {
    
    const loadCandidates = async () => {
        isLoading.value = true;
        error.value = null;
        try {
            const data = await getCandidateFromStorage();
            originalCandidates.value = data || [];
        } catch (e) {
            console.error('Lỗi khi tải dữ liệu ứng viên:', e);
            error.value = e.message;
        } finally {
            isLoading.value = false;
        }
    };

    const sortColumn = ref(null);
    const sortDirection = ref('asc');

    const handleSort = (columnKey) => {
        if (sortColumn.value === columnKey) {
            sortDirection.value = sortDirection.value === 'asc' ? 'desc' : 'asc';
        } else {
            sortColumn.value = columnKey;
            sortDirection.value = 'asc';
        }
    };

    // Tự động tính toán lại danh sách mỗi khi keyword/originalCandidates/sort thay đổi
    const filteredCandidates = computed(() => {
        const keyword = searchKeyword.value.toLowerCase().trim();
        let result = originalCandidates.value;

        if (keyword) {
            result = result.filter(c => 
                c.name?.toLowerCase().includes(keyword) ||
                c.phone?.toLowerCase().includes(keyword) ||
                c.email?.toLowerCase().includes(keyword)
            );
        }

        // Bắt đầu sắp xếp
        if (sortColumn.value) {
            result.sort((a, b) => {
                let valA = a[sortColumn.value] || '';
                let valB = b[sortColumn.value] || '';
                
                if (sortDirection.value === 'asc') {
                    return valA > valB ? 1 : (valA < valB ? -1 : 0);
                } else {
                    return valB > valA ? 1 : (valB < valA ? -1 : 0);
                }
            });
        }
        
        return result;
    });

    // add data to storage
    const addCandidate = async (newCandidate) => {
        try {
            // Tự tạo id nếu chưa có
            const candidateWithId = {
                ...newCandidate,
                id: Date.now() + '_' + Math.random().toString(36).substring(2, 9) // ID duy nhất dựa trên thời gian và random
            };
            originalCandidates.value.unshift(candidateWithId);
            await saveDataToStorage(originalCandidates.value);
        } catch (error) {
            console.error('Error adding candidate:', error);
            throw error;
        }
    }

    // Cập nhật ứng viên
    const updateCandidate = async (updatedCandidate) => {
        try {
            const index = originalCandidates.value.findIndex(c => c.id === updatedCandidate.id);
            if (index !== -1) {
                originalCandidates.value[index] = updatedCandidate;
                await saveDataToStorage(originalCandidates.value);
            }
        } catch (error) {
            console.error('Error updating candidate:', error);
            throw error;
        }
    }

    // Xóa ứng viên theo id
    const deleteCandidate = async(id) => {
        try {
            await deleteCandidateFromStorage(id); // Gọi hàm từ storage.js (không bị shadow)
            originalCandidates.value = originalCandidates.value.filter(c => c.id !== id);
        } catch (err) {
            console.error('Error deleting candidate:', err);
            throw err;
        }
    }

    return {
        candidates: filteredCandidates,
        allCandidates: originalCandidates,
        searchKeyword,
        isLoading,
        error,
        sortColumn,
        sortDirection,
        loadCandidates,
        addCandidate,
        updateCandidate,
        deleteCandidate,
        handleSort
    };
}