<template>
  <div class="ms-select-container" :class="{ 'has-error': error, 'is-open': isOpen, 'up': direction === 'up' }" v-click-outside="close">
    <label v-if="label" class="ms-label">
      {{ label }} <span v-if="required" class="required">*</span>
    </label>
    
    <div class="ms-select-wrapper" :class="{ 'is-disabled': disabled }" @click="toggle" tabindex="0" ref="wrapperRef" @keydown.enter.prevent="toggle" @keydown.space.prevent="toggle">
      <!-- Hiển thị giá trị đang chọn -->
      <div class="ms-select-display flex align-center justify-between">
        <span 
          class="selected-text" 
          :class="{ 'placeholder': !selectedLabel && selectedLabel !== 0 }"
          :title="selectedLabel"
        >
          {{ (selectedLabel || selectedLabel === 0) ? selectedLabel : placeholder }}
        </span>
        <!-- Icon mũi tên xuống -->
        <div class="ms-select-icon-wrapper flex align-center justify-center">
          <span class="ms-icon-base ms-icon--select-arrow ms-select-icon"></span>
        </div>
      </div>

      <!-- Popover danh sách option -->
      <transition name="ms-select-fade">
        <div v-if="isOpen" class="ms-select-dropdown box-shadow border-radius-4" :class="direction">
          <div 
            v-for="option in normalizedOptions" 
            :key="option.value"
            class="ms-select-item flex align-center justify-between"
            :class="{ 'selected': isOptionSelected(option.value) }"
            @click.stop="selectOption(option)"
          >
            <span class="item-text">{{ option.label }}</span>
            <div v-if="isOptionSelected(option.value)" class="ms-icon-base ms-icon--check dropdown-check"></div>
          </div>
        </div>
      </transition>
    </div>
    
    <span v-if="error" class="error-msg">{{ error }}</span>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue';

const props = defineProps({
  modelValue: [String, Number],
  label: String,
  placeholder: {
    type: String,
    default: ''
  },
  options: {
    type: Array,
    default: () => []
  },
  required: Boolean,
  disabled: Boolean,
  error: String,
  direction: {
    type: String,
    default: 'down',
    validator: (value) => ['up', 'down'].includes(value)
  }
});

const emit = defineEmits(['update:modelValue', 'change']);

// ── State ───────────────────────────────────────────────────
const isOpen = ref(false);

/** Chuẩn hóa mảng options */
const normalizedOptions = computed(() => {
  return props.options.map((option) => {
    if (typeof option === 'string' || typeof option === 'number') {
      return { value: option, label: option };
    }
    const value = option?.value ?? option?.label ?? '';
    const label = option?.label ?? option?.value ?? '';
    return { value, label };
  });
});

/** Nhãn của giá trị đang chọn */
const selectedLabel = computed(() => {
  if (props.modelValue === null || props.modelValue === undefined) return '';
  const found = normalizedOptions.value.find(opt => opt.value == props.modelValue);
  return found ? found.label : '';
});

/**
 * Compare option values safely.
 * - Avoid loose equality pitfalls like '' == 0 (causes multiple selections).
 * - Still allow matching numeric strings with numbers (e.g. '1' and 1).
 */
const isOptionSelected = (optionValue) => {
  if (props.modelValue === null || props.modelValue === undefined) return false;
  if (optionValue === null || optionValue === undefined) return false;

  // Strict match first
  if (props.modelValue === optionValue) return true;

  // If both are primitive, compare by string representation
  const a = typeof props.modelValue;
  const b = typeof optionValue;
  const isPrimitiveA = a === 'string' || a === 'number' || a === 'boolean';
  const isPrimitiveB = b === 'string' || b === 'number' || b === 'boolean';
  if (isPrimitiveA && isPrimitiveB) {
    return String(props.modelValue) === String(optionValue);
  }

  return false;
};

// ── Methods ──────────────────────────────────────────────────
const toggle = () => {
  if (props.disabled) return;
  isOpen.value = !isOpen.value;
};

const close = () => {
  isOpen.value = false;
};

const wrapperRef = ref(null);

const focus = () => {
  wrapperRef.value?.focus();
};

const selectOption = (option) => {
  emit('update:modelValue', option.value);
  emit('change', option.value);
  close();
};

defineExpose({ focus });

// ── Directives ───────────────────────────────────────────────
const vClickOutside = {
  mounted(el, binding) {
    el.clickOutsideEvent = function(event) {
      if (!(el === event.target || el.contains(event.target))) {
        binding.value(event);
      }
    };
    document.addEventListener('mousedown', el.clickOutsideEvent);
  },
  unmounted(el) {
    document.removeEventListener('mousedown', el.clickOutsideEvent);
  },
};
</script>

<style scoped>
.ms-select-container {
  display: flex;
  flex-direction: column;
  width: 100%;
  position: relative;
}

.ms-label {
  font-size: 14px;
  font-weight: 700;
  margin-bottom: 4px;
  color: #111;
}

.required {
  color: #eb5757;
}

/* ── Wrapper & Display ── */
.ms-select-wrapper {
  position: relative;
  width: 100%;
  height: 32px;
  cursor: pointer;
}

.ms-select-display {
  width: 100%;
  height: 100%;
  border: 1px solid #e0e0e0;
  border-radius: 2px; /* MISA standard is often quite sharp or 2px-3px */
  padding: 0 10px 0 12px;
  background-color: #fff;
  transition: all 0.2s;
  user-select: none;
}

.ms-select-display:hover {
  border-color: #2ca01c;
}

.is-open .ms-select-display {
  border-color: #2ca01c;
}

.selected-text {
  font-size: 13px;
  color: #111;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
  flex: 1;
}

.selected-text.placeholder {
  color: #999;
}

/* ── Dropdown List ── */
.ms-select-dropdown {
  position: absolute;
  left: 0;
  width: 100%;
  min-width: 100%;
  background-color: #fff;
  z-index: 2000;
  border: 1px solid #babec5;
  padding: 4px 1px;
  max-height: 200px;
  overflow-y: auto;
  box-shadow: 0 4px 12px rgba(0,0,0,0.15);
}

.ms-select-dropdown.down {
  top: 100%;
  margin-top: 2px;
  transform-origin: top;
}

.ms-select-dropdown.up {
  top: auto;
  bottom: 100%;
  margin-bottom: 2px;
  transform-origin: bottom;
  box-shadow: 0 -4px 12px rgba(0,0,0,0.15);
}

.ms-select-item {
  padding: 0 12px;
  height: 32px;
  font-size: 13px;
  color: #111;
  transition: all 0.1s;
  cursor: pointer;
  margin: 0 4px;
  border-radius: 2px;
}

.ms-select-item:hover {
  background-color: #f2f2f2;
  color: #2ca01c;
}

.ms-select-item.selected {
  background-color: #2ca01c;
  color: #fff;
}

.ms-select-item.selected .dropdown-check {
  background-color: #fff;
}

.item-text {
  white-space: nowrap;
}

.dropdown-check {
  width: 16px;
  height: 16px;
  background-color: #2ca01c;
  -webkit-mask-position: -121px -62px;
  mask-position: -121px -62px;
}

/* ── Icon ── */
.ms-select-icon-wrapper {
  width: 24px;
  height: 100%;
}

.ms-select-icon {
  background-color: #666;
  width: 16px;
  height: 16px;
  transition: transform 0.2s ease;
}

.is-open:not(.up) .ms-select-icon {
  transform: rotate(180deg);
}

.is-open.up .ms-select-icon {
  transform: rotate(180deg); /* Or 0 if you want it to point up when open downwards */
}

/* ── Transitions ── */
.ms-select-fade-enter-active,
.ms-select-fade-leave-active {
  transition: opacity 0.2s ease, transform 0.2s cubic-bezier(0.4, 0, 0.2, 1);
}

.ms-select-fade-enter-from,
.ms-select-fade-leave-to {
  opacity: 0;
  transform: scaleY(0.95);
}

/* ── States ── */
.is-disabled {
  opacity: 0.6;
  pointer-events: none;
  background-color: #f8f8f8;
}

.has-error .ms-select-display {
  border-color: #eb5757;
}

.error-msg {
  color: #eb5757;
  font-size: 12px;
  margin-top: 4px;
}

/* Custom scrollbar */
.ms-select-dropdown::-webkit-scrollbar {
  width: 6px;
}
.ms-select-dropdown::-webkit-scrollbar-thumb {
  background-color: #ccc;
  border-radius: 3px;
}
.ms-select-dropdown::-webkit-scrollbar-track {
  background-color: transparent;
}
</style>



