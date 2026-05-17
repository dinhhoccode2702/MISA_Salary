<template>
  <button
    class="ms-button flex align-center"
    :class="[type, size, { 'has-icon': iconClass, 'icon-only': isIconOnly }]"
    :style="{ backgroundColor: bgColor, color: textColor, borderColor: bgColor || '' }"
    :aria-label="ariaLabel || tooltip || undefined"
    :data-tooltip="tooltip || undefined"
    :disabled="disabled"
    @click="$emit('click')"
  >
    <span
      v-if="iconClass"
      class="ms-icon-base"
      :class="iconClass"
      :style="{ marginRight: isIconOnly ? '0' : '8px' }"
    ></span>
    <slot></slot>
  </button>
</template>

<script setup>
import { computed, useSlots } from 'vue';

const props = defineProps({
  type: {
    type: String,
    default: 'primary',
  },
  size: {
    type: String,
    default: 'medium',
  },
  iconClass: {
    type: String,
    default: '',
  },
  disabled: {
    type: Boolean,
    default: false,
  },
  bgColor: {
    type: String,
    default: '',
  },
  textColor: {
    type: String,
    default: '',
  },
  tooltip: {
    type: String,
    default: '',
  },
  ariaLabel: {
    type: String,
    default: '',
  },
});

const slots = useSlots();
const hasDefaultSlot = computed(() => {
  const nodes = slots.default?.() || [];
  return nodes.some((node) => String(node.children || '').trim());
});
const isIconOnly = computed(() => !!props.iconClass && !hasDefaultSlot.value);

defineEmits(['click']);
</script>

<style scoped>
.ms-button {
  border-radius: 4px;
  padding: 0 16px;
  font-weight: 600;
  font-size: 14px;
  justify-content: center;
  min-height: 32px;
  line-height: 1;
}

.primary {
  background-color: var(--color-primary);
  color: white;
}

.primary:hover {
  background-color: var(--color-primary-hover);
}

.primary:active {
  background-color: var(--color-primary-active);
}

.outline {
  background-color: transparent;
  border: 1px solid var(--border-color);
  color: var(--text-primary);
}

.outline:hover {
  background-color: #f5f5f5;
  border-color: var(--border-color-hover);
}

.ghost {
  background-color: transparent;
  color: var(--text-primary);
}

.ghost:hover {
  background-color: #f0f0f0;
}

.ms-button:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.small {
  padding: 0 8px;
  font-size: 12px;
  min-height: 24px;
}

.large {
  padding: 0 24px;
  font-size: 16px;
  min-height: 40px;
}

.icon-only {
  width: 32px;
  padding: 0;
}
</style>
