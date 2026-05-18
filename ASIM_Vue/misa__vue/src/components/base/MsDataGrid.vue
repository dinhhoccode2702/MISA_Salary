<template>
  <div ref="gridWrapperRef" class="ms-data-grid-wrapper" :class="gridWrapperClasses">
    <!-- Grid area (flex: 1) -->
    <div class="ms-grid-area">
        <DxDataGrid
          ref="dxDataGridRef"
          :data-source="dataSource"
          :remote-operations="remoteOperations"
          :show-borders="showBorders"
          :show-row-lines="showRowLines"
          :show-column-lines="showColumnLines"
          :column-auto-width="columnAutoWidth"
          :allow-column-resizing="allowColumnResizing"
          :column-resizing-mode="columnResizingMode"
          :allow-column-reordering="allowColumnReordering"
          :hover-state-enabled="hoverStateEnabled"
          :load-panel="{ enabled: false }"
          :no-data-text="noDataText"
          class="ms-grid"
          @row-click="$emit('row-click', $event)"
          @selection-changed="$emit('selection-changed', $event)"
          @cell-hover-changed="handleCellHoverChanged"
          @option-changed="handleOptionChanged"
        >
        <!-- Loading overlay -->
        <DxLoadPanel :enabled="true" :visible="loading" />

        <!-- Selection checkbox cột đầu tiên -->
        <DxSelection v-if="selection" v-bind="selection" />

        <!-- Scrolling mode -->
        <DxScrolling v-if="scrolling" v-bind="scrolling" />
        <DxColumnFixing :enabled="allowColumnFixing" />

        <!--
          Render các cột từ mảng columns.
          Mỗi col có thể có cellTemplate để dùng custom slot.
        -->
        <template v-for="col in visibleColumns" :key="col.dataField">
          <DxColumn
            :data-field="col.dataField"
            :caption="col.caption"
            :width="col.width"
            :min-width="col.minWidth"
            :max-width="col.maxWidth"
            :fixed="col.fixed"
            :fixed-position="col.fixedPosition || undefined"
            :visible-index="Number.isFinite(col.visibleIndex) ? col.visibleIndex : undefined"
            :alignment="col.alignment || 'left'"
            :cell-template="col.cellTemplate || undefined"
            :header-cell-template="col.headerCellTemplate || undefined"
            :sort-order="col.sortOrder || undefined"
            :allow-sorting="col.allowSorting !== false"
          />
        </template>

        <!--
          Kết nối slot tên động (named slots) với cell templates.
        -->
        <template
          v-for="col in columnsWithCellTemplate"
          :key="col.dataField + '-cell-tpl'"
          #[col.cellTemplate]="slotProps"
        >
          <slot :name="col.cellTemplate" :data="slotProps.data" />
        </template>

        <!--
          Kết nối slot tên động với header cell templates. (lặp qua những cột cần vẽ riêng  )
        -->
        <template
          v-for="col in columnsWithHeaderTemplate"
          :key="col.dataField + '-header-tpl'"
          #[col.headerCellTemplate]="slotProps"
        >
          <slot :name="col.headerCellTemplate" :data="slotProps.data" />
        </template>

        <!-- Slot mặc định – cho phép parent thêm DxColumn, DxSummary, v.v. trực tiếp -->
        <slot />

        <!-- Phân trang của DevExtreme (ẩn – ta dùng MsPager thay thế) -->
        <DxPaging v-if="paging" v-bind="paging" />
        <!-- Pager mặc định ẩn vì ta render MsPager tự viết bên ngoài -->
        <DxPager :visible="false" />
      </DxDataGrid>
    </div>

    <div
      v-if="hoveredActionColumn && hoveredRowSlotProps"
      class="ms-row-actions-overlay"
      :style="hoveredActionStyle"
      @mouseenter="handleActionOverlayEnter"
      @mouseleave="handleActionOverlayLeave"
    >
      <div class="ms-row-actions-overlay__content">
        <slot :name="hoveredActionColumn.cellTemplate" :data="hoveredRowSlotProps" />
      </div>
    </div>

    <!-- Pager tích hợp bên trong grid wrapper -->
    <div v-if="showPager" class="ms-grid-pager">
      <MsPager
        :total="total"
        :pageSize="pageSize"
        :currentPage="currentPage"
        :pageSizes="pageSizes"
        :direction="pagerDirection"
        @update:pageSize="$emit('update:pageSize', $event)"
        @update:currentPage="$emit('update:currentPage', $event)"
      />
    </div>
  </div>
</template>

<script setup>
import { computed, onBeforeUnmount, ref } from 'vue';
import {
  DxDataGrid,
  DxColumn,
  DxScrolling,
  DxSelection,
  DxPaging,
  DxPager,
  DxLoadPanel,
  DxColumnFixing,
} from 'devextreme-vue/data-grid';
import MsPager from './MsPager.vue';

const dxDataGridRef = ref(null);
const gridWrapperRef = ref(null);
const hoveredRowSlotProps = ref(null);
const hoveredRowTop = ref(0);
const isHoveringActionOverlay = ref(false);
let hoverClearTimer = null;
let columnStateTimer = null;

const actionColumns = computed(() =>
  props.columns.filter((col) => col.type === 'rowActions' && col.cellTemplate)
);

const hoveredActionColumn = computed(() => actionColumns.value[0] || null);

const hoveredActionStyle = computed(() => ({
  top: `${hoveredRowTop.value}px`,
}));

defineExpose({
  /** Clear all selected rows (checkboxes) in underlying DevExtreme grid */
  clearSelection: () => dxDataGridRef.value?.instance?.clearSelection?.(),
  /** Convenience getter for selected rows data */
  getSelectedRowsData: () => dxDataGridRef.value?.instance?.getSelectedRowsData?.(),
});

// ── Props ──────────────────────────────────────────────────
const props = defineProps({
  /** Dữ liệu hiển thị trong bảng (Array hoặc DevExtreme DataSource) */
  dataSource: {
    type: [Array, Object],
    default: () => [],
  },
  /**
   * Cấu hình cột. Mỗi phần tử:
   * {
   *   dataField: string,       – tên field trong data
   *   caption: string,         – tiêu đề cột
   *   width?: number,          – chiều rộng cố định
   *   minWidth?: number,       – chiều rộng tối thiểu
   *   maxWidth?: number,       – chiều rộng tối đa
   *   fixed?: boolean,         – cố định cột
   *   alignment?: string,      – căn chỉnh ('left'|'right'|'center')
   *   cellTemplate?: string,   – tên slot custom (phải match với slot trong parent)
   *   allowSorting?: boolean,  – cho phép sort (mặc định true)
   * }
   */
  columns: {
    type: Array,
    default: () => [],
  },
  /** Hiển thị loading spinner */
  loading: {
    type: Boolean,
    default: false,
  },
  /** Có dùng remote operations (server-side paging/sorting) không */
  remoteOperations: {
    type: Boolean,
    default: false,
  },
  /** Hiển thị viền bao ngoài grid */
  showBorders: {
    type: Boolean,
    default: false,
  },
  /** Hiển thị viền ngang giữa các dòng */
  showRowLines: {
    type: Boolean,
    default: true,
  },
  /** Hiển thị viền dọc giữa các cột */
  showColumnLines: {
    type: Boolean,
    default: false,
  },
  /** Tự động co dãn độ rộng cột theo nội dung */
  columnAutoWidth: {
    type: Boolean,
    default: true,
  },
  /** Cho phép kéo thả để thay đổi độ rộng cột */
  allowColumnResizing: {
    type: Boolean,
    default: true,
  },
  /** Chế độ resize cột */
  columnResizingMode: {
    type: String,
    default: 'widget',
  },
  /** Cho phép kéo thả để đổi thứ tự cột */
  allowColumnReordering: {
    type: Boolean,
    default: true,
  },
  /** Cho phép ghim cột bằng DevExtreme column fixing */
  allowColumnFixing: {
    type: Boolean,
    default: true,
  },
  /** Hiển thị highlight khi hover dòng */
  hoverStateEnabled: {
    type: Boolean,
    default: true,
  },
  /**
   * Cấu hình selection checkbox.
   * Truyền false để tắt selection.
   */
  selection: {
    type: [Object, Boolean],
    default: () => ({
      mode: 'multiple',
      showCheckBoxesMode: 'always',
      selectAllMode: 'allPages',
    }),
  },
  /** Cấu hình scrolling */
  scrolling: {
    type: Object,
    default: () => ({ mode: 'virtual' }),
  },
  /**
   * Cấu hình phân trang DevExtreme.
   * Mặc định tắt vì ta dùng MsPager tự viết.
   */
  paging: {
    type: [Object, Boolean],
    default: () => ({ enabled: false }),
  },
  /**
   * Màu nền header row.
   * - '#ffffff' → list chính (trắng)
   * - '#f4f5f8' → system dictionary (xám nhạt)
   */
  headerBg: {
    type: String,
    default: '#f6f6f6',
  },
  /** Text hiển thị khi không có dữ liệu */
  noDataText: {
    type: String,
    default: 'Không có dữ liệu',
  },

  // ── Pager Integration Props ──────────────────────────────
  /** Hiển thị pager tích hợp bên dưới grid */
  showPager: {
    type: Boolean,
    default: false,
  },
  /** Tổng số bản ghi */
  total: {
    type: Number,
    default: 0,
  },
  /** Số bản ghi mỗi trang (v-model) */
  pageSize: {
    type: Number,
    default: 25,
  },
  /** Trang hiện tại (v-model) */
  currentPage: {
    type: Number,
    default: 1,
  },
  /** Các option pageSize */
  pageSizes: {
    type: Array,
    default: () => [10, 25, 50, 100],
  },
  /** Hướng dropdown pager */
  pagerDirection: {
    type: String,
    default: 'up',
  },

  // ── Check Style Props ────────────────────────────────────
  /**
   * Kiểu hiển thị checkbox:
   * - 'checkbox' (mặc định): checkbox vuông DevExtreme gốc
   * - 'tick': dấu tick tròn xanh lá MISA
   */
  checkStyle: {
    type: String,
    default: 'checkbox',
    validator: (v) => ['checkbox', 'tick'].includes(v),
  },
});

// ── Emits ──────────────────────────────────────────────────
const emit = defineEmits([
  'row-click',
  'selection-changed',
  'update:pageSize',
  'update:currentPage',
  'columns-state-changed',
]);

// ── Computed ───────────────────────────────────────────────

/**
 * CSS classes cho wrapper dựa theo props.
 */
const gridWrapperClasses = computed(() => ({
  'with-column-lines': props.showColumnLines,
  'with-pager': props.showPager,
  'check-tick': props.checkStyle === 'tick',
}));

/**
 * Cột action được tách khỏi DxDataGrid và render ở overlay khi hover.
 */
const visibleColumns = computed(() =>
  props.columns.filter((col) => !(col.type === 'rowActions' && col.cellTemplate))
);

/**
 * Lọc các cột có cellTemplate để kết nối với named slot động.
 */
const columnsWithCellTemplate = computed(() =>
  visibleColumns.value.filter((col) => col.cellTemplate)
);

/**
 * Lọc các cột có headerCellTemplate để kết nối với named slot động.
 */
const columnsWithHeaderTemplate = computed(() => {
  const seen = new Set();
  return visibleColumns.value.filter((col) => {
    if (!col.headerCellTemplate || seen.has(col.headerCellTemplate)) {
      return false;
    }
    seen.add(col.headerCellTemplate);
    return true;
  });
});

const syncHoverOverlayPosition = (cellElement, rowData) => {
  const wrapperElement = gridWrapperRef.value;
  const rowElement = cellElement?.closest?.('.dx-row');

  if (!wrapperElement || !rowElement) return;

  const wrapperRect = wrapperElement.getBoundingClientRect();
  const rowRect = rowElement.getBoundingClientRect();

  hoveredRowSlotProps.value = { row: { data: rowData } };
  hoveredRowTop.value = rowRect.top - wrapperRect.top + rowRect.height / 2;
};

const clearHoverOverlay = () => {
  if (hoverClearTimer) {
    clearTimeout(hoverClearTimer);
    hoverClearTimer = null;
  }
  hoveredRowSlotProps.value = null;
  isHoveringActionOverlay.value = false;
};

const cancelHoverClear = () => {
  if (hoverClearTimer) {
    clearTimeout(hoverClearTimer);
    hoverClearTimer = null;
  }
};

const scheduleHoverClear = () => {
  cancelHoverClear();
  hoverClearTimer = setTimeout(() => {
    if (!isHoveringActionOverlay.value) {
      clearHoverOverlay();
    }
  }, 80);
};

const handleActionOverlayEnter = () => {
  isHoveringActionOverlay.value = true;
  cancelHoverClear();
};

const handleActionOverlayLeave = () => {
  clearHoverOverlay();
};

const handleCellHoverChanged = (e) => {
  if (!hoveredActionColumn.value || e.rowType !== 'data') {
    return;
  }

  if (e.eventType === 'mouseover') {
    cancelHoverClear();
    syncHoverOverlayPosition(e.cellElement, e.data);
    return;
  }

  if (e.eventType === 'mouseout' && !isHoveringActionOverlay.value) {
    const relatedOverlay = e.event?.relatedTarget?.closest?.('.ms-row-actions-overlay');
    if (relatedOverlay) {
      isHoveringActionOverlay.value = true;
      cancelHoverClear();
      return;
    }

    const relatedRow = e.event?.relatedTarget?.closest?.('.dx-row');
    const currentRow = e.cellElement?.closest?.('.dx-row');

    if (relatedRow !== currentRow) {
      scheduleHoverClear();
    }
  }
};

/**
 * Bước 2 trong luồng kéo thả cột.
 *
 * Ai gọi: scheduleColumnStateEmit().
 * Nhận vào: instance DevExtreme DataGrid qua biến component.
 * Việc làm:
 * - Đọc lại trạng thái thật của từng cột từ DevExtreme bằng columnOption(dataField).
 * - Lấy width để biết cột đang rộng bao nhiêu.
 * - Lấy fixed/fixedPosition để biết cột có đang ghim không.
 * - Lấy visibleIndex để biết cột đang nằm ở vị trí thứ mấy sau khi kéo.
 * Trả ra: mảng state đã sort theo visibleIndex, tức là đúng thứ tự cột đang thấy trên UI.
 */
const collectColumnState = (component) => {
  // DevExtreme giữ trạng thái cột bên trong instance.
  // Hàm này đọc lại trạng thái đó để parent có thể lưu vào store/DB.
  return visibleColumns.value.map((col, index) => {
    const option = component?.columnOption?.(col.dataField) || {};
    const fixedPosition = option.fixedPosition || col.fixedPosition || 'left';
    const width = option.width ?? option.visibleWidth ?? col.width;
    // visibleIndex là vị trí hiện tại của cột sau khi người dùng kéo thả.
    const visibleIndex = Number.isFinite(option.visibleIndex) ? option.visibleIndex : index;

    return {
      dataField: col.dataField,
      width,
      visible: option.visible !== false,
      fixed: !!option.fixed,
      fixedPosition,
      visibleIndex,
    };
  // Sort theo visibleIndex để mảng state phản ánh đúng thứ tự cột trên màn hình.
  }).sort((a, b) => a.visibleIndex - b.visibleIndex);
};

/**
 * Bước 3 trong luồng kéo thả cột.
 *
 * Ai gọi: handleOptionChanged().
 * Việc làm:
 * - Chờ sang tick tiếp theo bằng setTimeout(..., 0) để DevExtreme cập nhật xong state nội bộ.
 * - Gọi collectColumnState() để gom trạng thái cột mới nhất.
 * - Emit columns-state-changed lên component cha SalaryCompositionList.vue.
 *
 * Lý do có timer:
 * Khi kéo cột, DevExtreme có thể bắn option-changed trước khi mọi visibleIndex ổn định.
 * Delay 0ms giúp đọc state sau khi grid đã xử lý xong thao tác kéo.
 */
const scheduleColumnStateEmit = (component) => {
  if (columnStateTimer) {
    clearTimeout(columnStateTimer);
  }

  columnStateTimer = setTimeout(() => {
    columnStateTimer = null;
    const state = collectColumnState(component);
    emit('columns-state-changed', state);
  }, 0);
};

/**
 * Bước 1 trong luồng kéo thả cột.
 *
 * Ai gọi: DevExtreme DxDataGrid tự gọi khi option của grid thay đổi.
 * Trường hợp kéo cột: DevExtreme đổi option columns[x].visibleIndex.
 *
 * Việc làm:
 * - Bỏ qua các option không thuộc columns.
 * - Chỉ giữ các thay đổi có ý nghĩa lưu cấu hình như visibleIndex, width, fixed.
 * - Gọi scheduleColumnStateEmit() để gom state và báo lên màn cha.
 */
const handleOptionChanged = (e) => {
  // DevExtreme bắn rất nhiều option-changed; chỉ xử lý thay đổi thuộc columns.
  if (!e?.fullName?.startsWith?.('columns[')) {
    return;
  }

  // Chỉ các thay đổi này cần lưu cấu hình grid: kéo cột, resize, ẩn/hiện, ghim.
  const persistableChanges = ['.visibleIndex', '.width', '.visibleWidth', '.visible', '.fixed', '.fixedPosition'];
  if (!persistableChanges.some((key) => e.fullName.endsWith(key))) {
    return;
  }

  // Gom lại state mới rồi emit lên màn cha qua event columns-state-changed.
  scheduleColumnStateEmit(e.component);
};

onBeforeUnmount(() => {
  cancelHoverClear();
  if (columnStateTimer) {
    clearTimeout(columnStateTimer);
    columnStateTimer = null;
  }
});
</script>

<style scoped>
/* ── Wrapper bao ngoài ── */
.ms-data-grid-wrapper {
  height: 100%;
  flex: 1;
  min-height: 0;
  overflow: hidden;
  display: flex;
  flex-direction: column;
  position: relative;
}

/* ── Grid area: chiếm toàn bộ không gian còn lại ── */
.ms-grid-area {
  flex: 1;
  min-height: 0;
  overflow: hidden;
}

/* ── Grid DevExtreme ── */
.ms-grid {
  height: 100%;
}

/* ── Pager tích hợp ── */
.ms-grid-pager {
  flex-shrink: 0;
  height: 48px;
  padding: 0 16px;
  border-top: 1px solid #e0e0e0;
  display: flex;
  align-items: center;
  background-color: #f4f5f8; /* Đổi thành màu xám theo yêu cầu */
}

:deep(.ms-pager) {
  flex: 1;
  padding: 0 !important;
}

/* ── Header styles ──
   Dùng v-bind để đổi màu header theo prop headerBg.
   Chú ý dùng :deep() vì DevExtreme render ra ngoài scoped CSS.
*/

/* Ghi đè vào tận hàng và ô của Header */
:deep(.dx-datagrid-headers .dx-header-row > td) {
  background-color: #F5F5F5 !important;
  color: #111; /* Màu chữ tiêu đề */
  /* font-weight: 500; Đậm cho tiêu đề */
}
/* Đảm bảo cột Checkbox ở Header cũng ăn màu này */
:deep(.dx-datagrid-headers .dx-select-checkbox-column) {
  background-color: #F5F5F5 !important;
}

:deep(.dx-datagrid-headers) {
  background-color: #F5F5F5; /* Màu header từ prop */
  border-bottom: 1px solid #e0e0e0;
}

:deep(.dx-datagrid-headers .dx-datagrid-table .dx-header-row > td) {
  border-right: none;
  padding: 10px 16px;
  font-size: 13px;
  font-weight: 700;
  color: #111;
}

/* Khi showColumnLines = true thì thêm viền dọc */
.with-column-lines :deep(.dx-datagrid-headers .dx-datagrid-table .dx-header-row > td) {
  border-right: 1px solid #e0e0e0;
}

/* ── Body row styles ── */
:deep(.dx-datagrid-rowsview .dx-datagrid-table .dx-row > td) {
  border-right: none;
  padding: 10px 16px;
  font-size: 13px;
  vertical-align: middle;
}

/* Viền ngang mờ giữa các dòng */
:deep(.dx-datagrid-rowsview .dx-datagrid-table .dx-row) {
  border-bottom: 1px solid #670d0d;
}

/* ── Hover state ── */
:deep(.dx-row:hover) {
  cursor: pointer;
}

:deep(.dx-row:hover > td) {
  background-color: #f2f9f2 !important; /* Xanh nhạt khi hover */
}


.ms-row-actions-overlay {
  position: absolute;
  right: 12px;
  transform: translateY(-50%);
  z-index: 4;
  pointer-events: auto;
}

.ms-row-actions-overlay__content {
  pointer-events: auto;
}

/* ── Checkbox column (default) ── */
:deep(.dx-datagrid-checkbox-size) {
  font-size: 14px;
}

/* ── Loại bỏ border mặc định DxDataGrid ── */
:deep(.dx-datagrid) {
  border: none !important;
}

/* ==========================================================================
 * CUSTOM CHECKBOX BẢNG
 * (Đã thay thế bằng thiết kế vuông bo góc 4px, viền xám xanh của bạn)
 * ========================================================================== */

/* 1. Tùy chỉnh ô Checkbox gốc */
:deep(.dx-checkbox-icon) {
  height: 20px !important;
  width: 20px !important;
  background-color: #fff !important;
  border: 2px solid #8e95a5 !important; /* Viền xám xanh lúc bình thường */
  border-radius: 4px !important; /* Vuông bo góc 4px như bạn muốn */
  transition: all 0.2s ease;
  box-sizing: border-box !important;
}

/* Hiệu ứng khi di chuột qua dòng (hover) */
:deep(.dx-datagrid-table .dx-row:hover .dx-checkbox-icon),
:deep(.dx-checkbox:hover .dx-checkbox-icon) {
  border-color: var(--color-primary, #2ca04b) !important;
}

/* 2. Tùy chỉnh lúc đã click (Nền xanh lá) */
:deep(.dx-checkbox-checked .dx-checkbox-icon) {
  border-color: var(--color-primary, #2ca04b) !important;
  background-color: var(--color-primary, #2ca04b) !important;
}

/* 3. Tùy chỉnh dấu tick trắng */
/* Bước 3.1: Ẩn cục icon mặc định (xấu) của DevExtreme */
:deep(.dx-checkbox-icon::before) {
  display: none;
}

/* Bước 3.2: Vẽ lại dấu tick theo đúng thông số của thẻ .ms-checkbox-inner của bạn */
:deep(.dx-checkbox-checked .dx-checkbox-icon::before) {
  display: block;
  content: '';
  position: absolute;
  width: 5px;
  height: 10px;
  border: solid white;
  border-width: 0 2px 2px 0;
  transform: rotate(45deg);
  /* Điều chỉnh lại tọa độ để tâm dấu tick không bị lệch */
  top: 7px;
  left: 11px; 
}

/* Xử lý làm nổi bật nền của cả cái dòng (row) khi được chọn */
:deep(.dx-selection > td) {
  background-color: #f2f9f2 !important; /* Màu nền dòng xanh nhạt */
}

/* 4. XỬ LÝ THEO YÊU CẦU: Khi chưa chọn hết thì ô Header ĐỂ TRỐNG */
/* Ẩn dấu ô vuông/gạch ngang của trạng thái Indeterminate */
:deep(.dx-checkbox-indeterminate .dx-checkbox-icon::before) {
  display: none !important;
}
/* Đưa ô Indeterminate về màu trắng và viền xám như lúc chưa chọn */
:deep(.dx-checkbox-indeterminate .dx-checkbox-icon) {
  background-color: #fff !important;
  border-color: #8e95a5 !important;
}
/* 5. Highlight dòng được chọn */
:deep(.dx-selection > td) {
  background-color: #f2f9f2 !important;
}

/* 6. checkStyle = 'tick' (hiện tick màu xanh trên nền trắng)
.check-tick :deep(.dx-checkbox-icon) {
  background-color: #fff !important;
  border-color: var(--color-primary, #2ca04b) !important;
}

.check-tick :deep(.dx-datagrid-table .dx-row:hover .dx-checkbox-icon),
.check-tick :deep(.dx-checkbox:hover .dx-checkbox-icon) {
  border-color: var(--color-primary-hover, #35b324) !important;
}

.check-tick :deep(.dx-checkbox-checked .dx-checkbox-icon) {
  background-color: #fff !important;
  border-color: var(--color-primary, #2ca04b) !important;
}

/* Vẽ dấu tick màu xanh (override rule that hides default) */
/* .check-tick :deep(.dx-checkbox-checked .dx-checkbox-icon::before) {
  display: block;
  content: '';
  position: absolute;
  width: 6px;
  height: 10px;
  border: solid var(--color-primary, #2ca04b);
  border-width: 0 2px 2px 0;
  transform: rotate(45deg);
  top: 4px;
  left: 7px;
} */

</style>
