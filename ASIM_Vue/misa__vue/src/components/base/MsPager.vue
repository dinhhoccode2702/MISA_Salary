<template>
  <div class="ms-pager flex justify-between align-center">
    <div class="pager-left">
      Tổng số bản ghi: <strong>{{ total }}</strong>
    </div>

    <div class="pager-right flex align-center">
      <span class="pager-label m-r-8">Số bản ghi/trang</span>
      <div class="pager-select-wrapper m-r-16">
        <MsSelect
          :modelValue="pageSize"
          @update:modelValue="onPageSizeChange"
          :options="formattedPageSizes"
          :direction="direction"
        />
      </div>

      <span class="pager-range m-r-16">
        <strong>{{ rangeStart }} - {{ rangeEnd }}</strong> bản ghi
      </span>

      <div class="pager-nav flex align-center">
        <button
          class="pager-btn m-r-8"
          :class="{ disabled: currentPage <= 1 }"
          :disabled="currentPage <= 1"
          @click="prevPage"
          aria-label="Trang trước"
          data-tooltip="Trang trước"
        >
          <span class="ms-icon-base ms-icon--pager-prev pager-nav-icon"></span>
        </button>

        <button
          class="pager-btn"
          :class="{ disabled: currentPage >= totalPages }"
          :disabled="currentPage >= totalPages"
          @click="nextPage"
          aria-label="Trang sau"
          data-tooltip="Trang sau"
        >
          <span class="ms-icon-base ms-icon--pager-next pager-nav-icon"></span>
        </button>
      </div>
    </div>
  </div>
</template>

<script setup>
import { computed } from 'vue';
import MsSelect from './MsSelect.vue';

const props = defineProps({
  total: {
    type: Number,
    default: 0,
  },
  pageSize: {
    type: Number,
    default: 50,
  },
  currentPage: {
    type: Number,
    default: 1,
  },
  pageSizes: {
    type: Array,
    default: () => [10, 20, 30, 50, 100],
  },
  direction: {
    type: String,
    default: 'down',
  },
});

const emit = defineEmits(['update:pageSize', 'update:currentPage']);

const formattedPageSizes = computed(() =>
  props.pageSizes.map(size => ({
    value: size,
    label: `${size}`,
  }))
);

const totalPages = computed(() =>
  Math.max(1, Math.ceil(props.total / props.pageSize))
);

const rangeStart = computed(() =>
  props.total === 0 ? 0 : (props.currentPage - 1) * props.pageSize + 1
);

const rangeEnd = computed(() =>
  Math.min(props.currentPage * props.pageSize, props.total)
);

const onPageSizeChange = (val) => {
  const newSize = Number(val);
  emit('update:pageSize', newSize);
  emit('update:currentPage', 1);
};

const prevPage = () => {
  if (props.currentPage > 1) {
    emit('update:currentPage', props.currentPage - 1);
  }
};

const nextPage = () => {
  if (props.currentPage < totalPages.value) {
    emit('update:currentPage', props.currentPage + 1);
  }
};
</script>

<style scoped>
.ms-pager {
  padding: 0;
  height: 100%;
  font-size: 13px;
  color: #111;
  background-color: transparent;
}

.pager-left {
  font-weight: 400;
}

.pager-right {
  user-select: none;
}

.pager-select-wrapper {
  width: 90px;
}

.pager-btn {
  width: 24px;
  height: 24px;
  border: none;
  background-color: transparent;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 0;
  transition: all 0.2s;
  border-radius: 2px;
}

.pager-btn:hover:not(.disabled) {
  background-color: #e0e0e0;
}

.pager-nav-icon {
  background-color: #666;
  width: 20px;
  height: 20px;
}

.pager-btn.disabled {
  cursor: not-allowed;
  opacity: 0.5;
}

.pager-btn.disabled .pager-nav-icon {
  background-color: #bbb;
}

.m-r-8 { margin-right: 8px; }
.m-r-16 { margin-right: 16px; }
</style>
