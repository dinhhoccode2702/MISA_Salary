<template>
  <div class="salary-composition-list">
    <div class="main-view-container flex-column h-full">
      <MsPageHeader :title="R.PAGE_TITLE">
        <template #actions>
          <MsButton
            type="outline"
            icon-class="ms-icon--link-out"
            class="btn-system-custom"
            @click="$router.push('/system-dictionary')"
          >
            {{ R.BTN_SYSTEM_DICT }}
          </MsButton>

          <MsSplitButton
            class="m-l-8"
            icon-class="ms-icon--add"
            @click="openAddNewConfirm"
            :menu-items="addMenuItems"
            @select="handleAddMenuSelect"
          >
            {{ R.BTN_ADD_NEW }}
          </MsSplitButton>
        </template>
      </MsPageHeader>

      <div class="page-content flex-1 bg-white border-radius-4 box-shadow">

        <MsListToolbar
          v-model:searchValue="searchText"
          :search-placeholder="R.PLACEHOLDER_SEARCH"
          search-icon-class="ms-icon--search" 
          :filters="toolbarFilters"
          filters-align="right"
          :show-filter="true"                    
          :show-settings="true"                  
          :selected-count="salaryStore.selectedCount"
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

        <!-- Grid + Pager tích hợp bên trong -->
        <MsDataGrid
          :data-source="displayRows"
          :columns="gridColumns"
          :loading="salaryStore.loading"
          height="100%"
          :selection="{ mode: 'multiple', showCheckBoxesMode: 'always', selectAllMode: 'allPages' }"
          :show-pager="true"
          :total="salaryStore.pagination.total"
          v-model:pageSize="pageSize"
          v-model:currentPage="currentPage"
          :page-sizes="[10, 25, 50, 100]"
          pager-direction="up"
          ref="gridRef"
          @row-click="handleRowClick"
          @selection-changed="handleSelectionChanged"
        >
          <template #nameHeaderTemplate="{ data }">
            <div class="header-pin-cell flex align-center">
              <span class="header-text">{{ data.column.caption }}</span>
              <span class="ms-icon-base ms-icon--pin header-pin-icon m-l-4"></span>
            </div>
          </template>

          <template #natureTemplate="{ data }">
            <span>{{ data.value }}</span>
          </template>

          <template #valueTemplate="{ data }">
            <span v-if="data.value && data.value !== '-'" class="value-formula">
              <span class="value-equals">=</span>
              <span class="value-expression">{{ data.value }}</span>
            </span>
            <span v-else class="no-value">-</span>
          </template>

          <template #statusTemplate="{ data }">
            <div class="status-cell flex align-center">
              <span
                class="status-dot"
                :class="{ 'status-dot--inactive': Number(data.row?.data?.StatusCode) === 0 || data.value === 'Ngừng theo dõi' }"
              ></span>
              <span
                class="status-text"
                :class="{ 'status-text--inactive': Number(data.row?.data?.StatusCode) === 0 || data.value === 'Ngừng theo dõi' }"
              >{{ data.value }}</span>
            </div>
          </template>

          <template #actionTemplate="{ data }">
            <MsRowActions
              :actions="getRowActions(data.row.data)"
              @action="(key) => handleRowAction(key, data.row.data)"
            />
          </template>
        </MsDataGrid>

        <!-- Confirm/Notify Modal (centered) -->
        <MsModal
          :show="dialog.show"
          :title="dialog.title"
          width="560px"
          @close="closeDialog"
        >
          <div class="dialog-content">
            <div class="dialog-message">{{ dialog.message }}</div>
          </div>
          <template #footer>
            <MsButton
              v-for="btn in dialog.buttons"
              :key="btn.key"
              :type="btn.type"
              :bgColor="btn.bgColor || ''"
              :textColor="btn.textColor || ''"
              class="m-l-8"
              @click="btn.onClick"
            >
              {{ btn.label }}
            </MsButton>
          </template>
        </MsModal>

        <!-- Modal: Thêm từ danh mục của hệ thống -->
        <MsModal
          :show="showSystemImportModal"
          title="Thêm từ danh mục của hệ thống"
          width="1200px"
          @close="closeSystemImportModal"
        >
          <div class="sys-import-body">
            <div class="sys-import-toolbar flex align-center">
              <div class="sys-import-search">
                <MsInput
                  v-model="systemImportSearch"
                  placeholder="Tìm kiếm"
                  icon-class="ms-icon--search"
                />
              </div>
              <div class="sys-import-type">
                <MsSelect
                  v-model="systemImportType"
                  :options="systemImportTypeOptions"
                  placeholder="Tất cả thành phần"
                />
              </div>
            </div>

            <div class="sys-import-grid">
              <MsDataGrid
                ref="systemImportGridRef"
                :data-source="systemImportPagedRows"
                :columns="systemImportColumns"
                :loading="salaryStore.loading"
                height="100%"
                header-bg="#f4f5f8"
                :show-column-lines="false"
                :selection="{ mode: 'multiple', showCheckBoxesMode: 'always', selectAllMode: 'allPages' }"
                :show-pager="true"
                :total="systemImportTotal"
                v-model:pageSize="systemImportPagination.pageSize"
                v-model:currentPage="systemImportPagination.currentPage"
                :page-sizes="[10, 25, 50, 100]"
                pager-direction="up"
                @selection-changed="handleSystemImportSelectionChanged"
              />
            </div>
          </div>

          <template #footer>
            <MsButton type="outline" class="m-r-8" @click="closeSystemImportModal">Hủy bỏ</MsButton>
            <MsButton type="primary" :disabled="systemImportSelectedRows.length === 0" @click="submitSystemImport">Đồng ý</MsButton>
          </template>
        </MsModal>

      </div>

      <!-- Filter Drawer: Bộ lọc (right) -->
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
import { ref, onMounted, computed, watch } from 'vue';
import { useRouter } from 'vue-router';
import { useToast } from '@/composables/useToast';

import MsButton from '@/components/base/MsButton.vue';
import MsSplitButton from '@/components/base/MsSplitButton.vue';
import MsDataGrid from '@/components/base/MsDataGrid.vue';
import MsListToolbar from '@/components/base/MsListToolbar.vue';
import MsRowActions from '@/components/base/MsRowActions.vue';
import MsPageHeader from '@/components/layout/MsPageHeader.vue';
import MsModal from '@/components/common/MsModal.vue';
import MsInput from '@/components/base/MsInput.vue';
import MsCheckbox from '@/components/base/MsCheckbox.vue';
import MsSelect from '@/components/base/MsSelect.vue';

import { useSalaryStore } from '@/stores/salaryStore';
import { SALARY_COMPOSITION as R } from '@/utils/resources';

import organizationService from '@/services/organizationService';
import salaryService from '@/services/salaryService';

const router = useRouter();
const salaryStore = useSalaryStore();
const toast = useToast();

const addMenuItems = [
  { key: 'system-dictionary', label: 'Chọn từ danh mục của hệ thống' },
];

const handleAddMenuSelect = (key) => {
  if (key === 'system-dictionary') {
    openSystemImportModal();
  }
};

const showSystemImportModal = ref(false);
const systemImportSearch = ref('');
const systemImportType = ref('Tất cả thành phần');
const systemImportSelectedRows = ref([]);
const systemImportGridRef = ref(null);
const systemImportPagination = ref({ currentPage: 1, pageSize: 25 });

const systemImportTypeOptions = computed(() => {
  const base = ['Tất cả thành phần'];
  const types = new Set();
  (salaryStore.systemComponents || []).forEach((x) => {
    if (x?.SalaryCompositionType) types.add(x.SalaryCompositionType);
  });
  return [...base, ...Array.from(types)];
});

const systemImportColumns = computed(() => [
  { dataField: 'SalaryCompositionCode', caption: 'Mã thành phần', width: 220 },
  { dataField: 'SalaryCompositionName', caption: 'Tên thành phần', width: 320 },
  { dataField: 'SalaryCompositionType', caption: 'Loại thành phần', width: 220 },
  { dataField: 'Nature', caption: 'Tính chất', width: 140 },
  { dataField: 'TaxStatusText', caption: 'Chịu thuế', width: 160 },
  { dataField: 'TaxDeductionText', caption: 'Giảm trừ khi tính thuế', minWidth: 220 },
]);

const systemImportRows = computed(() => {
  const raw = Array.isArray(salaryStore.systemComponents) ? salaryStore.systemComponents : [];
  return raw.map((x) => {
    const natureType = Number(x?.salarySystemNatureType ?? x?.SalarySystemNatureType);
    const taxStatusText = natureType === 1 ? 'Chịu thuế' : (natureType === 2 ? 'Miễn thuế toàn phần' : '-');
    return {
      ...x,
      TaxStatusText: taxStatusText,
      TaxDeductionText: '-',
    };
  });
});

const systemImportFilteredRows = computed(() => {
  let rows = systemImportRows.value;

  const q = systemImportSearch.value.trim().toLowerCase();
  if (q) {
    rows = rows.filter((item) =>
      item.SalaryCompositionCode?.toLowerCase().includes(q) ||
      item.SalaryCompositionName?.toLowerCase().includes(q)
    );
  }

  if (systemImportType.value && systemImportType.value !== 'Tất cả thành phần') {
    rows = rows.filter((item) => item.SalaryCompositionType === systemImportType.value);
  }

  return rows;
});

const systemImportTotal = computed(() => systemImportFilteredRows.value.length);

const systemImportPagedRows = computed(() => {
  const pageSize = Number(systemImportPagination.value.pageSize) || 25;
  const currentPage = Number(systemImportPagination.value.currentPage) || 1;
  const start = (currentPage - 1) * pageSize;
  return systemImportFilteredRows.value.slice(start, start + pageSize);
});

watch([systemImportSearch, systemImportType], () => {
  systemImportPagination.value.currentPage = 1;
  systemImportSelectedRows.value = [];
  systemImportGridRef.value?.clearSelection?.();
});

const openSystemImportModal = async () => {
  showSystemImportModal.value = true;
  systemImportSearch.value = '';
  systemImportType.value = 'Tất cả thành phần';
  systemImportSelectedRows.value = [];
  systemImportPagination.value = { currentPage: 1, pageSize: 25 };

  await salaryStore.fetchSystemComponents();
};

const closeSystemImportModal = () => {
  showSystemImportModal.value = false;
  systemImportSelectedRows.value = [];
  systemImportGridRef.value?.clearSelection?.();
};

const handleSystemImportSelectionChanged = (e) => {
  systemImportSelectedRows.value = e.selectedRowsData || [];
};

const submitSystemImport = async () => {
  if (!systemImportSelectedRows.value.length) return;

  const orgId = salaryStore.filters.unit;
  if (!orgId) {
    showInfoDialog('Vui lòng chọn đơn vị áp dụng trước khi thêm từ danh mục hệ thống.');
    return;
  }

  const systemIds = systemImportSelectedRows.value
    .map((x) => x?.salarySystemId ?? x?.SalarySystemId)
    .filter(Boolean);

  if (!systemIds.length) {
    showInfoDialog('Không xác định được ID hệ thống để import.');
    return;
  }

  toast.confirm({
    message: `Bạn có muốn thêm ${systemIds.length} thành phần từ danh mục hệ thống không?`,
    cancelLabel: 'Hủy',
    okLabel: 'Đồng ý',
  }).then(async (ok) => {
    if (!ok) return;
    try {
      const response = await salaryService.bulkImport({ SystemIds: systemIds, OrganizationId: orgId });
      const insertedCount = Number(getServiceData(response)) || 0;
      closeSystemImportModal();
      await salaryStore.fetchSalaryCompositions();
      if (insertedCount > 0) {
        toast.show(`Đã thêm ${insertedCount} thành phần từ danh mục hệ thống.`, 'success');
      } else {
        showInfoDialog('Các thành phần đã chọn đã tồn tại trong danh sách sử dụng.');
      }
    } catch (err) {
      console.error('Bulk import error:', err);
      showInfoDialog(getErrorMessage(err));
    }
  });
};

const dialog = ref({
  show: false,
  title: 'Thông báo',
  message: '',
  buttons: [],
});

const closeDialog = () => {
  dialog.value = { show: false, title: 'Thông báo', message: '', buttons: [] };
};

const showInfoDialog = (message, title = 'Thông báo') => {
  dialog.value = {
    show: true,
    title,
    message,
    buttons: [
      { key: 'close', label: 'Đóng', type: 'primary', onClick: closeDialog },
    ],
  };
};

const showConfirmDialog = ({ title = 'Thông báo', message, cancelLabel = 'Hủy', okLabel = 'Đồng ý', okVariant = 'primary', onOk }) => {
  const okButton = {
    key: 'ok',
    label: okLabel,
    type: okVariant === 'danger' ? 'primary' : 'primary',
    bgColor: okVariant === 'danger' ? 'var(--color-error)' : '',
    onClick: async () => {
      closeDialog();
      await onOk?.();
    }
  };

  dialog.value = {
    show: true,
    title,
    message,
    buttons: [
      { key: 'cancel', label: cancelLabel, type: 'outline', onClick: closeDialog },
      okButton,
    ],
  };
};

const getErrorMessage = (err) =>
  err?.response?.data?.userMsg ||
  err?.response?.data?.UserMsg ||
  err?.response?.data?.devMsg ||
  err?.response?.data?.DevMsg ||
  'Có lỗi xảy ra, vui lòng thử lại.';

const getServiceData = (response) => {
  const payload = response?.data ?? response;
  return payload?.data ?? payload?.Data ?? payload;
};

const isSystemSalaryComposition = (row) =>
  Number(row?.SalaryCompositionIsSystemStatus) === 1 ||
  row?.Source === 'Hệ thống' ||
  row?.NguonTao === 'Mặc định';


const searchText = ref('');

const showColumnSettings = ref(false);

// Column settings draft state
const columnSettingsSearch = ref('');
const columnVisibilityDraft = ref({});

const openColumnSettings = () => {
  // Snapshot current visibility
  const draft = {};
  salaryStore.columns.forEach((c) => {
    draft[c.dataField] = !!c.visible;
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
  const cols = Array.isArray(salaryStore.columns) ? salaryStore.columns : [];
  if (!q) return cols;
  return cols.filter((c) => (c.caption || '').toLowerCase().includes(q));
});

const saveColumnSettings = () => {
  const draft = columnVisibilityDraft.value || {};
  salaryStore.columns.forEach((c) => {
    const nextVisible = !!draft[c.dataField];
    if (c.visible !== nextVisible) {
      salaryStore.updateColumnVisibility(c.dataField, nextVisible);
    }
  });
  closeColumnSettings();
};

const pageSize = computed({
  get: () => salaryStore.pagination.pageSize,
  set: (val) => {
    salaryStore.setPagination({ pageSize: Number(val) || 50, currentPage: 1 });
    salaryStore.fetchSalaryCompositions();
  }
});

const currentPage = computed({
  get: () => salaryStore.pagination.currentPage,
  set: (val) => {
    salaryStore.setPagination({ currentPage: Number(val) || 1 });
    salaryStore.fetchSalaryCompositions();
  }
});

const isActiveRow = (rowData) => Number(rowData?.StatusCode) !== 0 && rowData?.Status !== 'Ngừng theo dõi';

const getRowActions = (rowData) => {
  const statusAction = isActiveRow(rowData)
    ? { key: 'stop', icon: 'stop', label: 'Ngừng theo dõi', variant: 'warning' }
    : { key: 'follow', icon: 'stop', label: 'Theo dõi lại', variant: 'default' };

  return [
    statusAction,
    { key: 'copy', icon: 'copy', label: 'Nhân bản', variant: 'default' },
    { key: 'edit', icon: 'edit', label: 'Sửa', variant: 'default' },
    { key: 'delete', icon: 'delete', label: 'Xóa', variant: 'danger' },
  ];
};

const gridRef = ref(null);

const selectionToolbarActions = [
  { key: 'delete', label: 'Xóa', icon: 'delete', variant: 'danger' },
];

const openAddNewConfirm = () => {
  toast.confirm({
    message: 'Bạn có muốn thêm thành phần lương mới không?',
    cancelLabel: 'Hủy',
    okLabel: 'Đồng ý',
  }).then((ok) => {
    if (ok) router.push('/salary-composition/add');
  });
};

/** Cấu hình filter dropdowns cho MsListToolbar */
const toolbarFilters = ref([
  {
    id: 'status',
    modelValue: '',
    options: [
      { value: '', label: R.OPTION_ALL_STATUS },
      { value: 1, label: 'Đang theo dõi' },
      { value: 0, label: 'Ngừng theo dõi' }
    ],
    width: '150px', // Thu ngắn ô Trạng thái
  },
  {
    id: 'unit',
    modelValue: '',
    options: [{ value: '', label: R.OPTION_ALL_UNITS_FILTER }],
    width: '350px', // Kéo dài ô Đơn vị
  },
]);

const isFilterDrawerOpen = ref(false);
const filterFieldSearch = ref('');

// Filterable fields for drawer (checkbox list)
const FILTER_FIELDS = [
  { key: 'SalaryCompositionCode', label: 'Mã thành phần' },
  { key: 'SalaryCompositionName', label: 'Tên thành phần' },
  { key: 'SalaryCompositionType', label: 'Loại thành phần' },
  { key: 'AppliedUnit', label: 'Đơn vị áp dụng' },
  { key: 'Nature', label: 'Tính chất' },
  { key: 'KieuGiaTri', label: 'Kiểu giá trị' },
  { key: 'GiaTri', label: 'Giá trị' },
  { key: 'NguonTao', label: 'Nguồn tạo' },
  { key: 'Status', label: 'Hiển thị trên phiếu lương' },
];

const DEFAULT_FILTER_FIELD_KEYS = ['SalaryCompositionCode', 'SalaryCompositionName'];

const appliedFilterFieldKeys = ref([...DEFAULT_FILTER_FIELD_KEYS]);
const filterFieldsDraft = ref({});

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

const clearFilterFields = () => {
  const draft = {};
  FILTER_FIELDS.forEach((f) => {
    draft[f.key] = DEFAULT_FILTER_FIELD_KEYS.includes(f.key);
  });
  filterFieldsDraft.value = draft;
  appliedFilterFieldKeys.value = [...DEFAULT_FILTER_FIELD_KEYS];
  closeFilterDrawer();
};

const applyFilterFields = () => {
  const draft = filterFieldsDraft.value || {};
  const next = FILTER_FIELDS.filter((f) => !!draft[f.key]).map((f) => f.key);
  appliedFilterFieldKeys.value = next.length ? next : [...DEFAULT_FILTER_FIELD_KEYS];
  closeFilterDrawer();
};

let searchTimeout;
watch(searchText, (newVal) => {
  clearTimeout(searchTimeout);
  searchTimeout = setTimeout(() => {
    salaryStore.setSearchText(newVal);
    salaryStore.fetchSalaryCompositions();
  }, 500);
});

// Rows displayed in grid (client-side narrowing based on applied filter field selection)
const displayRows = computed(() => {
  return Array.isArray(salaryStore.salaryCompositions) ? salaryStore.salaryCompositions : [];
});

onMounted(async () => {
  try {
    const orgRes = await organizationService.getAll();
    const orgService = orgRes?.data;
    const orgs = orgService?.Data ?? orgService?.data ?? orgService ?? orgRes ?? [];
    const unitFilter = toolbarFilters.value.find(f => f.id === 'unit');
    if (unitFilter && Array.isArray(orgs)) {
      unitFilter.options = [
        { value: '', label: R.OPTION_ALL_UNITS_FILTER },
        ...orgs.map(o => ({ value: o.organizationId, label: o.organizationName }))
      ];
    }
  } catch (error) {
    console.error('Lỗi khi tải danh sách tổ chức:', error);
  }

  salaryStore.fetchSalaryCompositions();
});


/**
 */
const gridColumns = computed(() => {
  const cols = salaryStore.visibleColumns.map((col) => {
    const newCol = { ...col, alignment: 'left' };
    
    if (col.dataField === 'SalaryCompositionCode') {
      newCol.fixed = true;
    }
    if (col.dataField === 'SalaryCompositionName') {
      newCol.fixed = true;
      newCol.headerCellTemplate = 'nameHeaderTemplate';
    }
    
    if (col.dataField === 'Nature') {
      newCol.cellTemplate = 'natureTemplate';
    }
    
    return newCol;
  });

  cols.push({
    type: 'rowActions',
    cellTemplate: 'actionTemplate',
  });

  return cols;
});


const handleRowClick = (e) => {
  if (e.event?.target?.closest('.row-actions')) {
    return;
  }
  
  const id = e.data?.SalaryCompositionId;
  if (id) {
    router.push(`/salary-composition/${id}`);
  }
};

const handleSelectionChanged = (e) => {
  salaryStore.setSelectedRows(e.selectedRowsData || []);
};

const handleDeselect = () => {
  salaryStore.setSelectedRows([]);
  gridRef.value?.clearSelection?.();
};

/** Action khi có selection (toolbar) */
const handleSelectionAction = async (key) => {
  if (key !== 'delete') return;
  const rows = salaryStore.selectedRows || [];
  if (rows.length === 0) return;

  const ok = await toast.confirm({
    message: 'Bạn có chắc chắn muốn xóa các thành phần lương đã chọn không?',
    cancelLabel: 'Hủy',
    okLabel: 'Xóa',
    okVariant: 'danger',
  });
  if (!ok) return;

  try {
    const ids = rows.map((r) => r.SalaryCompositionId).filter(Boolean);
    await salaryStore.deleteSalaryCompositions(ids);
    handleDeselect();
    toast.show(`Đã xóa ${ids.length} thành phần lương.`, 'success');
  } catch (err) {
    showInfoDialog(getErrorMessage(err));
  }
};

/** Xử lý action từ MsRowActions */
const handleRowAction = async (key, rowData) => {
  const id = rowData?.SalaryCompositionId;
  if (!id) {
    showInfoDialog('Không xác định được thành phần lương cần thao tác.');
    return;
  }

  if (key === 'edit') {
    router.push(`/salary-composition/${id}`);
  } else if (key === 'copy') {
    router.push({ path: `/salary-composition/${id}`, query: { mode: 'clone' } });
  } else if (key === 'delete') {
    toast.confirm({
      message: `Bạn có chắc chắn muốn xóa thành phần lương ${rowData.SalaryCompositionName} không?`,
      cancelLabel: 'Hủy',
      okLabel: 'Xóa',
      okVariant: 'danger',
    }).then(async (ok) => {
      if (!ok) return;
      try {
        await salaryStore.deleteSalaryComposition(id);
        toast.show('Đã xóa thành phần lương.', 'success');
      } catch (err) {
        showInfoDialog(getErrorMessage(err));
      }
    });
  } else if (key === 'stop' || key === 'follow') {
    const nextStatus = key === 'follow' ? 1 : 0;
    const nextText = nextStatus === 1 ? 'theo dõi lại' : 'ngừng theo dõi';
    const ok = await toast.confirm({
      message: `Bạn có muốn ${nextText} thành phần lương ${rowData.SalaryCompositionName} không?`,
      cancelLabel: 'Hủy',
      okLabel: 'Đồng ý',
    });
    if (!ok) return;

    try {
      await salaryStore.updateSalaryCompositionStatus(id, nextStatus);
      toast.show(`Đã ${nextText} thành phần lương.`, 'success');
    } catch (err) {
      showInfoDialog(getErrorMessage(err));
    }
  }
};

const handleFilterChange = ({ id, value }) => {
  const target = toolbarFilters.value.find((f) => f.id === id);
  if (target) target.modelValue = value;
  salaryStore.setFilter(id, value);
  salaryStore.fetchSalaryCompositions();
};

/** Toggle Advanced Filter */
// (Filter icon now opens drawer UI)

const refreshData = () => {
  salaryStore.fetchSalaryCompositions();
};
</script>

<style scoped>
.salary-composition-list {
  display: flex;
  flex-direction: column;
  flex: 1;
  height: 100%;
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
  border-radius: 4px;
  position: relative;
}

.btn-system-custom {
  background-color: #ffffff !important; 
  font-weight: 500 !important;          
  border-color: #e0e0e0 !important;     
  color: #333 !important;               
}

.btn-system-custom:hover {
  border-color: #41b929 !important; 
  color: #41b929 !important;        
  background-color: #ffffff !important; 
}

.btn-system-custom:hover :deep(.ms-icon--link-out) {
  background-color: #41b929 !important; 
}

.nature-cell {
  width: 100%;
}

.header-pin-cell {
  height: 100%;
}

.header-pin-icon {
  background-color: #888;
  opacity: 0.6;
}

.advanced-filter-panel {
  padding: 12px 16px;
  background-color: #fafafa;
  border-bottom: 1px solid #e0e0e0;
}

.filter-label {
  font-weight: 500;
  color: #333;
}

.clear-filters-btn {
  color: #2ca01c;
  cursor: pointer;
  font-weight: 500;
  transition: color 0.15s;
}
.clear-filters-btn:hover {
  color: #248b17;
  text-decoration: underline;
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

.sys-import-body {
  display: flex;
  flex-direction: column;
  height: 68vh;
  min-height: 520px;
}

.sys-import-toolbar {
  gap: 12px;
  margin-bottom: 12px;
}

.sys-import-search {
  width: 320px;
}

.sys-import-type {
  width: 220px;
}

.sys-import-grid {
  flex: 1;
  min-height: 0;
  border: 1px solid #e0e0e0;
  border-radius: 4px;
  overflow: hidden;
}
</style>

<style scoped>
/* Formula & value rendering */
.value-formula {
  font-weight: 500;
}

.value-equals {
  color: var(--color-error);
  margin-right: 2px;
}

.value-expression {
  color: var(--text-formula);
}

.no-value {
  color: var(--text-secondary);
}

/* Status rendering */
.status-cell {
  gap: 8px;
}

.status-dot {
  width: 8px;
  height: 8px;
  border-radius: 50%;
  background-color: var(--color-success);
}

.status-dot--inactive {
  background-color: var(--text-secondary);
}

.status-text {
  color: var(--text-link);
}

.status-text--inactive {
  color: var(--text-secondary);
}

.dialog-content {
  font-size: 13px;
  color: var(--text-primary);
}

.dialog-message {
  line-height: 1.5;
}
</style>
