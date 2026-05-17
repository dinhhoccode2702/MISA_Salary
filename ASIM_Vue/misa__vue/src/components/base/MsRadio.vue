<template>
  <div class="ms-radio-container" :class="{ 'has-error': error }">
    <label v-if="label" class="ms-label">{{ label }} <span v-if="required" class="required">*</span></label>
    <div class="ms-radio-group flex gap-24">
      <label v-for="option in options" :key="option.value" class="ms-radio-item flex align-center cursor-pointer">
        <input 
          type="radio" 
          :name="name" 
          :value="option.value" 
          :checked="modelValue === option.value"
          @change="$emit('update:modelValue', option.value)"
          class="ms-radio-input"
          :disabled="disabled"
        />
        <span class="ms-radio-circle"></span>
        <span class="ms-radio-label">{{ option.label }}</span>
      </label>
    </div>
    <span v-if="error" class="error-msg">{{ error }}</span>
  </div>
</template>

<script setup>
defineProps({
  modelValue: [String, Number, Boolean],
  label: String,
  options: {
    type: Array,
    required: true
  },
  name: {
    type: String,
    required: true
  },
  required: Boolean,
  disabled: Boolean,
  error: String
});

defineEmits(['update:modelValue']);
</script>

<style scoped>
.ms-radio-container {
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

.required {
  color: #eb5757;
}

.ms-radio-group {
  padding: 8px 0;
}

.ms-radio-item {
  position: relative;
  user-select: none;
}

.ms-radio-input {
  position: absolute;
  opacity: 0;
  cursor: pointer;
  height: 0;
  width: 0;
}

.ms-radio-circle {
  height: 18px;
  width: 18px;
  background-color: #fff;
  border: 2px solid #bdbdbd;
  border-radius: 50%;
  display: inline-block;
  margin-right: 8px;
  transition: border-color 0.2s, background-color 0.2s;
  position: relative;
}

.ms-radio-item:hover .ms-radio-circle {
  border-color: var(--color-primary);
}

.ms-radio-input:checked ~ .ms-radio-circle {
  border-color: var(--color-primary);
}

.ms-radio-circle::after {
  content: '';
  position: absolute;
  display: none;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  width: 8px;
  height: 8px;
  border-radius: 50%;
  background: var(--color-primary);
}

.ms-radio-input:checked ~ .ms-radio-circle::after {
  display: block;
}

.ms-radio-label {
  font-size: 14px;
  color: #111;
}

/* ── Error Styles ── */
.has-error .error-msg {
  color: #eb5757;
  font-size: 12px;
  margin-top: 4px;
}
</style>
