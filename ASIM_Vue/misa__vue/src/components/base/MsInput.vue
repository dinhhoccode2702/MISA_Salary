<template>
  <div class="ms-input-container" :class="{ 'has-error': error }">
    <label v-if="label" class="ms-label">{{ label }} <span v-if="required" class="required">*</span></label>
    <div class="ms-input-wrapper flex align-center" :class="{ 'has-icon-left': iconClass && iconLeft, 'has-icon-right': iconClass && !iconLeft, 'is-disabled': disabled }">
      <span v-if="iconClass && iconLeft" class="ms-icon-base" :class="iconClass" style="margin-right: 8px;"></span>
      <input 
        ref="inputRef"
        :type="type" 
        :value="displayValue" 
        :placeholder="placeholder"
        :disabled="disabled"
        @input="handleInput"
        @keyup.enter="$emit('enter')"
        @focus="isFocused = true; $emit('focus')"
        @blur="handleBlur"
        :class="['ms-input', inputClass]"
      />
      <span v-if="iconClass && !iconLeft" class="ms-icon-base" :class="iconClass" style="margin-left: 8px;"></span>
    </div>
    <span v-if="error" class="error-msg">{{ error }}</span>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue';
const props = defineProps({
  modelValue: [String, Number],
  label: String,
  placeholder: String,
  type: { type: String, default: 'text' },
  iconClass: String,
  iconLeft: { type: Boolean, default: true },
  error: String,
  required: Boolean,
  disabled: Boolean,
  inputClass: String,
  format: { type: String, default: '' } // 'currency' | 'number' | ''
});

const inputRef = ref(null);
const isFocused = ref(false);

const focus = () => {
  inputRef.value?.focus();
};

const displayValue = computed(() => {
  if (!props.modelValue && props.modelValue !== 0) return '';
  if (isFocused.value) return props.modelValue; // Show raw when editing
  
  if (props.format === 'currency' || props.format === 'number') {
    return Number(props.modelValue).toLocaleString('vi-VN');
  }
  return props.modelValue;
});

const handleInput = (event) => {
  let val = event.target.value;
  if (props.format === 'currency' || props.format === 'number') {
    val = val.replace(/\D/g, ''); // Keep only digits
    emit('update:modelValue', val ? Number(val) : '');
  } else {
    emit('update:modelValue', val);
  }
};

const handleBlur = (e) => {
  isFocused.value = false;
  emit('blur', e);
};

defineExpose({ focus });
const emit = defineEmits(['update:modelValue', 'enter', 'focus', 'blur']);
</script>

<style scoped>
.ms-input-container {
  display: flex;
  flex-direction: column;
  width: 100%;
}

.ms-label {
  font-size: 13px;
  font-weight: 700;
  margin-bottom: 4px;
  color: #111;
}

.required { color: #eb5757; }

.ms-input-wrapper {
  border: 1px solid #e0e0e0;
  border-radius: 4px;
  padding: 0 12px;
  height: 32px;
  background-color: white;
  transition: border-color 0.2s;
  display: flex;
  align-items: center;
}

.ms-input-wrapper:hover {
  border-color: var(--border-color-hover);
}

.ms-input-wrapper:focus-within {
  border-color: var(--color-primary);
}

.ms-input-wrapper.is-disabled {
  background-color: #f3f4f6;
  border-color: #e0e0e0;
  cursor: not-allowed;
}

.ms-input {
  border: none;
  width: 100%;
  height: 100%;
  font-size: 13px;
  outline: none;
  background-color: transparent;
}

.ms-input:disabled {
  cursor: not-allowed;
}

.ms-input::placeholder {
  font-style: normal; /* Đã đổi từ italic sang normal theo ảnh mẫu */
  color: #999;
}

/* Kích thước icon trong input – webkit-mask được kế thừa từ ms-icon-base trong icons.css */
/* Không cần định nghĩa lại ở đây vì ms-icon-base đã handle display/flex-shrink */

.has-icon-left .ms-input {
  padding-left: 0;
}

.has-icon-right .ms-input {
  padding-right: 0;
}

.has-error .ms-input-wrapper {
  border-color: #eb5757;
}

.error-msg {
  color: #eb5757;
  font-size: 12px;
  margin-top: 4px;
}
</style>
