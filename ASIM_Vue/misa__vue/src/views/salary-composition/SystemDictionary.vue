<template>
  <div class="system-dictionary">
    <div class="main-view-container flex-column h-full">
      <MsPageHeader :title="R.SYSTEM_DICT_TITLE" show-back @back="$router.push('/salary-composition')" />

      <div class="page-content flex-1 bg-white border-radius-4 box-shadow">
        <MsListToolbar
          v-model:searchValue="searchText"
          :search-placeholder="R.PLACEHOLDER_SEARCH"
          search-icon-class="ms-icon--search"
          :filters="toolbarFilters"
          filters-align="right"
          :show-filter="true"
          :show-settings="true"
          :selected-count="selectedRows.length"
          :selection-actions="selectionToolbarActions"
          @filter-change="handleFilterChange"
          @filter="openFilterDrawer"
          @settings="openColumnSettings"
          @deselect="handleDeselect"
          @selection-action="handleSelectionAction"
        />

        <div v-if="showColumnSettings" class="column-settings-overlay" @mousedown.self="closeColumnSettings">
          <div class="column-settings-popover">
            <div class="popover-header flex align-center">
              <div class="popover-title">Tùy chỉnh cột</div>
              <div class="popover-close" @click="closeColumnSettings">×</div>
            </div>

            <MsInput
              v-model="columnSettingsSearch"
              placeholder="Tìm kiếm"
              icon-class="ms-icon--search"
              class="popover-search"
            />

            <div class="popover-list">
              <MsCheckbox
                v-for="col in filteredColumnOptions"
                :key="col.dataField"
                v-model="columnVisibilityDraft[col.dataField]"
                :label="col.caption"
              />
            </div>

            <div class="popover-footer">
              <MsButton type="primary" class="btn-save" @click="saveColumnSettings">Lưu</MsButton>
            </div>
          </div>
        </div>

        <MsDataGrid
          :data-source="pagedComponents"
          :columns="gridColumns"
          :loading="salaryStore.loading"
          height="100%"
          header-bg="#f4f5f8"
          :show-column-lines="false"
          check-style="tick"
          :selection="{ mode: 'multiple', showCheckBoxesMode: 'always', selectAllMode: 'allPages' }"
          :show-pager="true"
          :total="filteredComponents.length"
          v-model:pageSize="pagination.pageSize"
          v-model:currentPage="pagination.currentPage"
          :page-sizes="[10, 25, 50, 100]"
          pager-direction="up"
          ref="gridRef"
          @selection-changed="handleSelectionChanged"
        >
          <template #formulaTemplate="{ data }">
            <span v-if="data.value" class="formula-text">{{ data.value.startsWith('=') ? '' : '=' }}{{ data.value }}</span>
            <span v-else class="no-value">-</span>
          </template>
          <template #actionTemplate="{ data }">
            <MsRowActions :actions="rowActions" @action="(key) => handleRowAction(key, data.row.data)" />
          </template>
        </MsDataGrid>
      </div>

      <div v-if="isFilterDrawerOpen" class="filter-drawer-overlay" @mousedown.self="closeFilterDrawer">
        <div class="filter-drawer">
          <div class="filter-drawer-header flex align-center">
            <div class="filter-drawer-title">Bộ lọc</div>
            <div class="filter-drawer-close" @click="closeFilterDrawer">×</div>
          </div>

          <MsInput
            v-model="filterFieldSearch"
            placeholder="Tìm kiếm"
            icon-class="ms-icon--search"
            class="filter-drawer-search"
          />

          <div class="filter-drawer-list">
            <MsCheckbox
              v-for="f in filteredFilterFields"
              :key="f.key"
              v-model="filterFieldsDraft[f.key]"
              :label="f.label"
            />
          </div>

          <div class="filter-drawer-footer flex align-center justify-end">
            <MsButton type="outline" class="m-r-8" @click="clearFilterFields">Bỏ lọc</MsButton>
            <MsButton type="primary" @click="applyFilterFields">Áp dụng</MsButton>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive, computed, onMounted, watch } from 'vue';
import { useRouter } from 'vue-router';
import MsDataGrid from '@/components/base/MsDataGrid.vue';
import MsListToolbar from '@/components/base/MsListToolbar.vue';
import MsRowActions from '@/components/base/MsRowActions.vue';
import MsPageHeader from '@/components/layout/MsPageHeader.vue';
import MsButton from '@/components/base/MsButton.vue';
import MsInput from '@/components/base/MsInput.vue';
import MsCheckbox from '@/components/base/MsCheckbox.vue';
import { useSalaryStore } from '@/stores/salaryStore';
import salaryService from '@/services/salaryService';
import { SALARY_COMPOSITION as R } from '@/utils/resources';
import { useToast } from '@/composables/useToast';

const router = useRouter();
const salaryStore = useSalaryStore();
const toast = useToast();

const searchText = ref('');
const selectedRows = ref([]);
const showColumnSettings = ref(false);
const gridRef = ref(null);
const pagination = reactive({ currentPage: 1, pageSize: 25 });

const columnSettingsSearch = ref('');
const columnVisibilityDraft = ref({});
const columnDefs = ref([
  { dataField: 'SalaryCompositionCode', caption: 'Mã thành phần', width: 200, visible: true },
  { dataField: 'SalaryCompositionName', caption: 'Tên thành phần', width: 250, visible: true },
  { dataField: 'SalaryCompositionType', caption: 'Loại thành phần', width: 200, visible: true },
  { dataField: 'Nature', caption: 'Tính chất', width: 120, visible: true },
  { dataField: 'ValueType', caption: 'Kiểu giá trị', width: 120, visible: true },
  { dataField: 'Value', caption: 'Giá trị', minWidth: 200, cellTemplate: 'formulaTemplate', visible: true },
]);

const isFilterDrawerOpen = ref(false);
const filterFieldSearch = ref('');
const filterFieldsDraft = ref({});

const FILTER_FIELDS = [
  { key: 'SalaryCompositionCode', label: 'Mã thành phần' },
  { key: 'SalaryCompositionName', label: 'Tên thành phần' },
  { key: 'SalaryCompositionType', label: 'Loại thành phần' },
  { key: 'Nature', label: 'Tính chất' },
  { key: 'ValueType', label: 'Kiểu giá trị' },
  { key: 'Value', label: 'Giá trị' },
];
const DEFAULT_FILTER_FIELD_KEYS = ['SalaryCompositionCode', 'SalaryCompositionName'];
const appliedFilterFieldKeys = ref([...DEFAULT_FILTER_FIELD_KEYS]);

const typeOptions = [
  'Tất cả thành phần',
  'Lương',
  'Phụ cấp',
  'Giảm trừ',
  'Bảo hiểm - Công đoàn',
  'Thuế TNCN',
  'Chấm công',
  'Thông tin nhân viên',
];
const selectedType = ref('Tất cả thành phần');
const toolbarFilters = ref([{ id: 'type', modelValue: selectedType.value, options: typeOptions, width: '220px' }]);
const rowActions = [{ key: 'add', icon: 'add', color: 'green', label: 'Thêm', variant: 'default', noHoverBg: true }];
const selectionToolbarActions = [
  { key: 'add-to-list', label: 'Đưa vào danh sách sử dụng', icon: 'add' },
];

const openColumnSettings = () => {
  const draft = {};
  columnDefs.value.forEach((c) => {
    draft[c.dataField] = c.visible !== false;
  });
  columnVisibilityDraft.value = draft;
  columnSettingsSearch.value = '';
  showColumnSettings.value = true;
};

const closeColumnSettings = () => {
  showColumnSettings.value = false;
};

const filteredColumnOptions = computed(() => {
  const q = columnSettingsSearch.value.trim().toLowerCase();
  if (!q) return columnDefs.value;
  return columnDefs.value.filter((c) => (c.caption || '').toLowerCase().includes(q));
});

const saveColumnSettings = () => {
  const draft = columnVisibilityDraft.value || {};
  columnDefs.value.forEach((c) => {
    c.visible = !!draft[c.dataField];
  });
  closeColumnSettings();
};

const openFilterDrawer = () => {
  const draft = {};
  FILTER_FIELDS.forEach((f) => {
    draft[f.key] = appliedFilterFieldKeys.value.includes(f.key);
  });
  filterFieldsDraft.value = draft;
  filterFieldSearch.value = '';
  isFilterDrawerOpen.value = true;
};

const closeFilterDrawer = () => {
  isFilterDrawerOpen.value = false;
};

const filteredFilterFields = computed(() => {
  const q = filterFieldSearch.value.trim().toLowerCase();
  if (!q) return FILTER_FIELDS;
  return FILTER_FIELDS.filter((f) => f.label.toLowerCase().includes(q));
});

const resetGridState = () => {
  pagination.currentPage = 1;
  selectedRows.value = [];
  gridRef.value?.clearSelection?.();
};

const clearFilterFields = () => {
  const draft = {};
  FILTER_FIELDS.forEach((f) => {
    draft[f.key] = DEFAULT_FILTER_FIELD_KEYS.includes(f.key);
  });
  filterFieldsDraft.value = draft;
  appliedFilterFieldKeys.value = [...DEFAULT_FILTER_FIELD_KEYS];
  resetGridState();
  closeFilterDrawer();
};

const applyFilterFields = () => {
  const draft = filterFieldsDraft.value || {};
  const next = FILTER_FIELDS.filter((f) => !!draft[f.key]).map((f) => f.key);
  appliedFilterFieldKeys.value = next.length ? next : [...DEFAULT_FILTER_FIELD_KEYS];
  resetGridState();
  closeFilterDrawer();
};

const gridColumns = computed(() => {
  const cols = columnDefs.value
    .filter((c) => c.visible !== false)
    .map((c) => ({ ...c }));

  cols.push({
    type: 'rowActions',
    cellTemplate: 'actionTemplate',
  });

  return cols;
});

const filteredComponents = computed(() => {
  let rows = Array.isArray(salaryStore.systemComponents) ? salaryStore.systemComponents : [];

  if (selectedType.value !== 'Tất cả thành phần') {
    rows = rows.filter((item) => item.SalaryCompositionType === selectedType.value);
  }

  const q = searchText.value.trim().toLowerCase();
  if (q) {
    const keys = appliedFilterFieldKeys.value.length ? appliedFilterFieldKeys.value : DEFAULT_FILTER_FIELD_KEYS;
    rows = rows.filter((item) => keys.some((k) => String(item?.[k] ?? '').toLowerCase().includes(q)));
  }

  return rows;
});

const pagedComponents = computed(() => {
  const pageSize = Number(pagination.pageSize) || 25;
  const currentPage = Number(pagination.currentPage) || 1;
  const start = (currentPage - 1) * pageSize;
  return filteredComponents.value.slice(start, start + pageSize);
});

const getSystemId = (row) => row?.salarySystemId || row?.SalarySystemId;

const importSystemRows = async (rows) => {
  const systemIds = rows.map(getSystemId).filter(Boolean);
  if (!systemIds.length) {
    toast.show('Không xác định được thành phần hệ thống cần thêm.', 'error');
    return;
  }

  const organizationId = salaryStore.filters.unit;
  if (!organizationId) {
    toast.show('Vui lòng chọn đơn vị áp dụng ở danh sách Thành phần lương trước khi thêm.', 'warning');
    return;
  }

  await salaryService.bulkImport({ systemIds, organizationId });
  await salaryStore.fetchSalaryCompositions();
  toast.show(`Đã thêm ${systemIds.length} thành phần vào danh sách sử dụng.`, 'success');
  router.push('/salary-composition');
};

const handleFilterChange = ({ id, value }) => {
  if (id === 'type') {
    selectedType.value = value;
  }
  const filter = toolbarFilters.value.find((x) => x.id === id);
  if (filter) filter.modelValue = value;
  resetGridState();
};

const handleRowAction = (key, data) => {
  if (key !== 'add') return;
  toast.confirm({
    message: `Bạn có muốn thêm thành phần "${data?.SalaryCompositionName || ''}" vào danh sách sử dụng không?`,
    cancelLabel: 'Hủy',
    okLabel: 'Đồng ý',
  }).then(async (ok) => {
    if (!ok) return;
    try {
      await importSystemRows([data]);
    } catch (error) {
      console.error('[SystemDictionary] import row error:', error);
      toast.show('Không thể thêm thành phần vào danh sách sử dụng. Vui lòng thử lại.', 'error');
    }
  });
};

const handleSelectionChanged = (e) => {
  selectedRows.value = e.selectedRowsData || [];
};

const handleDeselect = () => {
  selectedRows.value = [];
  gridRef.value?.clearSelection?.();
};

const handleSelectionAction = (key) => {
  if (key === 'add-to-list') handleSelect();
};

const handleSelect = () => {
  if (!selectedRows.value.length) return;
  toast.confirm({
    message: `Bạn có muốn đưa ${selectedRows.value.length} thành phần vào danh sách sử dụng không?`,
    cancelLabel: 'Hủy',
    okLabel: 'Đồng ý',
  }).then(async (ok) => {
    if (!ok) return;
    try {
      await importSystemRows(selectedRows.value);
    } catch (error) {
      console.error('[SystemDictionary] import selected rows error:', error);
      toast.show('Không thể thêm các thành phần vào danh sách sử dụng. Vui lòng thử lại.', 'error');
    }
  });
};

watch([searchText, selectedType], resetGridState);
watch(() => pagination.pageSize, resetGridState);

onMounted(() => {
  salaryStore.fetchSystemComponents().catch(() => {
    toast.show('Không thể tải danh mục hệ thống. Vui lòng thử lại.', 'error');
  });
});
</script>

<style scoped>
.system-dictionary {
  display: flex;
  flex-direction: column;
  height: 100vh;
  overflow: hidden;
  background-color: #efefef;
}

.page-content {
  flex: 1;
  min-height: 0;
  margin: 0 24px 24px 24px;
  display: flex;
  flex-direction: column;
  overflow: hidden;
  background-color: #fff;
  border: 1px solid #e0e0e0;
  position: relative;
}

:deep(.ms-list-toolbar) {
  flex-shrink: 0;
  border-bottom: 1px solid #f2f2f2;
}

.formula-text {
  color: #0050b3;
  font-weight: 500;
}

.no-value {
  color: #999;
}

.column-settings-overlay {
  position: absolute;
  inset: 0;
  z-index: 50;
  background: rgba(0, 0, 0, 0);
}

.column-settings-popover {
  position: absolute;
  top: 56px;
  right: 12px;
  width: 340px;
  max-height: calc(100% - 68px);
  background: #fff;
  border: 1px solid #e0e0e0;
  border-radius: 4px;
  box-shadow: 0 8px 24px rgba(0, 0, 0, 0.12);
  display: flex;
  flex-direction: column;
}

.popover-header {
  padding: 12px;
  border-bottom: 1px solid #e0e0e0;
  justify-content: space-between;
}

.popover-title {
  font-weight: 600;
  color: #111;
}

.popover-close {
  width: 28px;
  height: 28px;
  border-radius: 4px;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  user-select: none;
  color: #666;
}

.popover-close:hover {
  background: #f5f5f5;
}

.popover-search {
  padding: 12px;
}

.popover-list {
  padding: 0 12px 12px 12px;
  overflow: auto;
  flex: 1;
}

.popover-footer {
  padding: 12px;
  border-top: 1px solid #e0e0e0;
}

.btn-save {
  width: 100%;
}

.filter-drawer-overlay {
  position: fixed;
  inset: 0;
  z-index: 80;
  background: rgba(0, 0, 0, 0.15);
  display: flex;
  justify-content: flex-end;
}

.filter-drawer {
  width: 360px;
  height: 100%;
  background: #fff;
  box-shadow: -8px 0 24px rgba(0, 0, 0, 0.12);
  display: flex;
  flex-direction: column;
}

.filter-drawer-header {
  padding: 14px 16px;
  border-bottom: 1px solid #e0e0e0;
  justify-content: space-between;
}

.filter-drawer-title {
  font-weight: 600;
  color: #111;
}

.filter-drawer-close {
  width: 28px;
  height: 28px;
  border-radius: 4px;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  user-select: none;
  color: #666;
}

.filter-drawer-close:hover {
  background: #f5f5f5;
}

.filter-drawer-search {
  padding: 12px 16px;
}

.filter-drawer-list {
  padding: 0 16px;
  overflow: auto;
  flex: 1;
}

.filter-drawer-footer {
  padding: 12px 16px;
  border-top: 1px solid #e0e0e0;
}
</style>
