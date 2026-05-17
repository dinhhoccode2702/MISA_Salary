<template>
  <div class="ms-list-toolbar flex align-center" :class="{ 'has-selection': selectedCount > 0 }">

    <!-- â”€â”€ BÃªn trÃ¡i: Search â”€â”€ -->
    <div class="toolbar-left flex align-center">

      <!-- Ã” tÃ¬m kiáº¿m (luÃ´n hiá»ƒn thá»‹) -->
      <MsInput
        v-if="showSearch"
        :model-value="searchValue"
        :placeholder="searchPlaceholder"
        :icon-class="searchIconClass"
        class="toolbar-search"
        @update:model-value="$emit('update:searchValue', $event)"
      />

      <!-- â”€â”€ Selection Info (khi cÃ³ dÃ²ng Ä‘Æ°á»£c chá»n) â”€â”€ -->
      <template v-if="selectedCount > 0">
        <div class="selection-info flex align-center">
          <span class="selection-label">
            Đã chọn <strong class="selection-count">{{ selectedCount }}</strong>
          </span>
          <span class="selection-divider"></span>
          <span class="selection-deselect" @click="$emit('deselect')">Bỏ chọn</span>

          <!-- Action buttons khi cÃ³ selection -->
          <template v-for="action in selectionActions" :key="action.key">
            <span v-if="action.variant !== 'danger'" class="selection-divider"></span>
            <button
              type="button"
              class="selection-action flex align-center"
              :class="{ 'selection-action--danger': action.variant === 'danger' }"
              @click="$emit('selection-action', action.key)"
            >
              <span
                v-if="action.icon"
                class="ms-icon-base selection-action-icon"
                :class="`ms-icon--${action.icon}`"
              ></span>
              {{ action.label }}
            </button>
          </template>
        </div>
      </template>
    </div>

    <!-- â”€â”€ BÃªn pháº£i: Filters + Icons (chá»‰ hiá»‡n khi KHÃ”NG cÃ³ selection) â”€â”€ -->
    <div v-if="selectedCount === 0" class="toolbar-right flex align-center">

      <!-- Filters bÃªn pháº£i (máº·c Ä‘á»‹nh theo screenshot MISA) -->
      <div
        v-if="filters.length && filtersAlign === 'right'"
        class="toolbar-filters flex align-center"
      >
        <MsSelect
          v-for="filter in filters"
          :key="filter.id"
          :model-value="filter.modelValue"
          :options="filter.options"
          :class="['toolbar-select', (filter.id === 'status' || filter.id === 'type') ? 'no-border' : '']"
          :style="{ width: filter.width || '180px' }"
          @update:model-value="$emit('filter-change', { id: filter.id, value: $event })"
        />
      </div>

      <!-- NÃºt lá»c nÃ¢ng cao (icon phá»…u) -->
      <div
        v-if="showFilter"
        class="utility-icon-btn"
        role="button"
        aria-label="Lọc"
        data-tooltip="Lọc"
        data-tooltip-position="bottom"
        @click="$emit('filter')"
      >
        <!-- Icon filter â€“ webkit-mask -->
        <span class="ms-icon-base ms-icon--filter utility-icon"></span>
      </div>

      <!-- NÃºt lÃ m má»›i -->
      <div
        v-if="showRefresh"
        class="utility-icon-btn"
        role="button"
        aria-label="Làm mới"
        data-tooltip="Làm mới"
        data-tooltip-position="bottom"
        @click="$emit('refresh')"
      >
        <span class="ms-icon-base ms-icon--refresh utility-icon"></span>
      </div>

      <!-- NÃºt xuáº¥t kháº©u (áº©n máº·c Ä‘á»‹nh) -->
      <div
        v-if="showExport"
        class="utility-icon-btn"
        role="button"
        aria-label="Xuất khẩu"
        data-tooltip="Xuất khẩu"
        data-tooltip-position="bottom"
        @click="$emit('export')"
      >
        <span class="ms-icon-base ms-icon--export utility-icon"></span>
      </div>

      <!-- NÃºt tÃ¹y chá»‰nh cá»™t -->
      <div
        v-if="showSettings"
        class="utility-icon-btn"
        role="button"
        aria-label="Tùy chỉnh"
        data-tooltip="Tùy chỉnh"
        data-tooltip-position="bottom"
        @click="$emit('settings')"
      >
        <span class="ms-icon-base ms-icon--col-settings utility-icon"></span>
      </div>
    </div>

    <!-- Filters bÃªn trÃ¡i (náº¿u align=left, chá»‰ khi khÃ´ng cÃ³ selection) -->
    <div
      v-if="filters.length && filtersAlign === 'left' && selectedCount === 0"
      class="toolbar-filters flex align-center"
    >
      <MsSelect
        v-for="filter in filters"
        :key="filter.id"
        :model-value="filter.modelValue"
        :options="filter.options"
        class="toolbar-select"
        :style="{ width: filter.width || '180px' }"
        @update:model-value="$emit('filter-change', { id: filter.id, value: $event })"
      />
    </div>
  </div>
</template>

<script setup>
import MsInput from './MsInput.vue';
import MsSelect from './MsSelect.vue';

// â”€â”€ Props â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
defineProps({
  /** GiÃ¡ trá»‹ hiá»‡n táº¡i cá»§a Ã´ search (dÃ¹ng v-model:searchValue) */
  searchValue: {
    type: String,
    default: '',
  },
  /** Placeholder cá»§a Ã´ search */
  searchPlaceholder: {
    type: String,
    default: 'Tìm kiếm',
  },
  /** Class icon trong Ã´ search */
  searchIconClass: {
    type: String,
    default: 'icon-search-gray',
  },
  /** Hiá»ƒn thá»‹ Ã´ search khÃ´ng */
  showSearch: {
    type: Boolean,
    default: true,
  },
  /**
   * Máº£ng filter dropdowns.
   * Má»—i pháº§n tá»­: { id, modelValue, options, width? }
   */
  filters: {
    type: Array,
    default: () => [],
  },
  /**
   * Vá»‹ trÃ­ Ä‘áº·t filter dropdowns.
   * 'right' â†’ Ä‘áº·t á»Ÿ bÃªn pháº£i (máº·c Ä‘á»‹nh theo MISA design)
   * 'left'  â†’ Ä‘áº·t á»Ÿ bÃªn trÃ¡i cáº¡nh search
   */
  filtersAlign: {
    type: String,
    default: 'left',
  },
  /** Hiá»ƒn thá»‹ nÃºt lá»c nÃ¢ng cao (icon phá»…u) */
  showFilter: {
    type: Boolean,
    default: true,
  },
  /** Hiá»ƒn thá»‹ nÃºt lÃ m má»›i */
  showRefresh: {
    type: Boolean,
    default: false,
  },
  /** Hiá»ƒn thá»‹ nÃºt xuáº¥t kháº©u */
  showExport: {
    type: Boolean,
    default: false,
  },
  /** Hiá»ƒn thá»‹ nÃºt tÃ¹y chá»‰nh cá»™t */
  showSettings: {
    type: Boolean,
    default: true,
  },

  // â”€â”€ Selection Info Props â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
  /** Sá»‘ dÃ²ng Ä‘ang Ä‘Æ°á»£c chá»n. Khi > 0, toolbar chuyá»ƒn sang selection mode */
  selectedCount: {
    type: Number,
    default: 0,
  },
  /** CÃ¡c action button hiá»‡n khi cÃ³ selection */
  selectionActions: {
    type: Array,
    default: () => [],
  },
});

// â”€â”€ Emits â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
defineEmits([
  'update:searchValue',
  'filter-change',
  'filter',
  'refresh',
  'export',
  'settings',
  'deselect',
  'selection-action',
]);
</script>

<style scoped>
/* â”€â”€ Container toolbar â”€â”€ */
.ms-list-toolbar {
  padding: 12px 16px;
  gap: 12px;
  border-bottom: 1px solid #f0f0f0; /* Viá»n má» phÃ¢n cÃ¡ch vá»›i grid */
  justify-content: space-between;
}

/* â”€â”€ BÃªn trÃ¡i â”€â”€ */
.toolbar-left {
  gap: 10px;
  flex: 1;
}

/* Ã” search cá»‘ Ä‘á»‹nh chiá»u rá»™ng */
.toolbar-search {
  width: 240px;
  flex-shrink: 0;
}

/* â”€â”€ Selection Info â”€â”€ */
.selection-info {
  gap: 0;
  font-size: 13px;
  color: #111;
}

.selection-label {
  font-weight: 400;
  white-space: nowrap;
}

.selection-count {
  color: #2ca01c;
  font-weight: 700;
  font-size: 14px;
}

.selection-divider {
  width: 1px;
  height: 16px;
  background-color: #d0d0d0;
  margin: 0 12px;
  flex-shrink: 0;
}

.selection-deselect {
  color: #2ca01c;
  cursor: pointer;
  font-weight: 500;
  white-space: nowrap;
  transition: color 0.15s;
}
.selection-deselect:hover {
  color: #248b17;
  text-decoration: underline;
}

.selection-action {
  color: #2ca01c;
  cursor: pointer;
  font-weight: 500;
  white-space: nowrap;
  gap: 4px;
  transition: color 0.15s;
  border: none;
  background: transparent;
  padding: 0;
}
.selection-action:hover {
  color: #248b17;
  text-decoration: underline;
}

.selection-action-icon {
  width: 16px;
  height: 16px;
  background-color: #2ca01c;
}

.selection-action--danger {
  color: var(--color-error);
  border: 1px solid var(--color-error);
  border-radius: 4px;
  padding: 6px 12px;
  margin-left: 12px;
  text-decoration: none;
}

.selection-action--danger:hover {
  color: var(--color-error);
  text-decoration: none;
}

.selection-action--danger .selection-action-icon {
  background-color: var(--color-error);
}

/* â”€â”€ BÃªn pháº£i â”€â”€ */
.toolbar-right {
  gap: 6px; /* Khoáº£ng cÃ¡ch nhá» giá»¯a cÃ¡c icon */
  flex-shrink: 0;
}

/* Filters inline */
.toolbar-filters {
  gap: 8px;
  margin-right: 6px; /* Khoáº£ng cÃ¡ch giá»¯a dropdowns vÃ  icon buttons */
}

.toolbar-select {
  min-width: 180px;
}

/* CHá»ˆ xÃ³a viá»n cá»§a Ã´ Tráº¡ng thÃ¡i (id: status) - nhÆ°ng váº«n giá»¯ viá»n nháº¹ khi hover */
.toolbar-select.no-border :deep(.ms-select) {
  border-color: transparent;
  padding-left: 0;
}

.toolbar-select.no-border :deep(.ms-select:hover) {
  border-color: transparent;
  background-color: #f8f8f8;
}

/* Khi hover vÃ o Ã´ select trong toolbar */
.toolbar-select :deep(.ms-select:hover) {
  background-color: #f8f8f8;
}

.toolbar-select.no-border :deep(.ms-select:hover) {
  background-color: transparent; /* Ã” khÃ´ng viá»n thÃ¬ khÃ´ng cáº§n highlight ná»n */
  color: var(--color-primary);
}

/* â”€â”€ Icon button wrapper â”€â”€ */
.utility-icon-btn {
  width: 24px;
  height: 24px;
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: 4px;
  cursor: pointer;
  transition: all 0.15s;
  flex-shrink: 0;
  padding: 4px; /* Táº¡o khoáº£ng Ä‘á»‡m cho icon bÃªn trong */
}

/* Hover effect cho icon button */
.utility-icon-btn:hover {
  background-color: #f5f5f5;
}

/* Ã‰p mÃ u icon khi hover vÃ o wrapper - DÃ¹ng mÃ u xanh MISA */
.utility-icon-btn:hover .utility-icon {
  background-color: var(--color-primary) !important; 
}

/* â”€â”€ Icon mask color & base â”€â”€ */
.utility-icon {
  background-color: #666;
  width: 24px;
  height: 24px;
  transition: background-color 0.15s;
}

/* CHá»ˆ xÃ³a viá»n cá»§a Ã´ Tráº¡ng thÃ¡i (id: status hoáº·c type) */
.toolbar-select.no-border :deep(.ms-select-display) {
  border-color: transparent !important;
  background-color: transparent !important;
  padding-left: 0;
}

.toolbar-select.no-border :deep(.ms-select-display:hover) {
  background-color: #f8f8f8 !important;
}

.toolbar-select.no-border :deep(.selected-text) {
  color: #212121;
  font-weight: 500;
}

/* Khi má»Ÿ dropdown: Chuyá»ƒn sang mÃ u xanh MISA vÃ  font dÃ y hÆ¡n */
.toolbar-select.no-border.is-open :deep(.selected-text) {
  color: var(--color-primary);
  font-weight: 700;
}

.toolbar-select.no-border.is-open :deep(.ms-select-icon) {
  background-color: var(--color-primary);
}
</style>

