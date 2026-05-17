<template>
  <Transition name="toast">
    <div v-if="visible" class="ms-toast" :class="type">
      <div class="toast-main flex align-center">
        <div v-if="showIcon" class="toast-icon m-r-8" :class="iconClass"></div>
        <div class="toast-content">
          <span class="toast-text">{{ message }}</span>
        </div>
        <div v-if="dismissible" class="toast-close" @click="close">×</div>
      </div>

      <!-- Confirm actions (2 buttons, aligned left) -->
      <div v-if="hasActions" class="toast-actions flex align-center">
        <MsButton
          v-for="action in actions"
          :key="action.key"
          :type="action.type || 'primary'"
          size="small"
          :bgColor="action.bgColor || ''"
          :textColor="action.textColor || ''"
          class="m-r-8" 
          @click="handleAction(action)"
        >
          {{ action.label }}
        </MsButton>
      </div>
    </div>
  </Transition>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import MsButton from '@/components/base/MsButton.vue';

const props = defineProps({
  id: [String, Number],
  message: String,
  type: { type: String, default: 'success' }, // success, error, warning, info, confirm
  duration: { type: Number, default: 3000 },
  dismissible: { type: Boolean, default: true },
  actions: { type: Array, default: null },
  onClose: { type: Function, default: null },
});

const visible = ref(true);

const iconClass = {
  success: 'icon-success',
  error: 'icon-error-toast',
  warning: 'icon-warning-toast',
  info: 'icon-info-toast',
  confirm: 'icon-info-toast',
}[props.type];

const showIcon = props.type !== 'confirm';
const hasActions = Array.isArray(props.actions) && props.actions.length > 0;
const actions = props.actions || [];

const close = () => {
  visible.value = false;
  props.onClose?.();
};

const handleAction = async (action) => {
  try {
    await action?.onClick?.();
  } catch (e) {
    // action handler decides how to notify; keep toast UI stable
    console.error(e);
  }
};

onMounted(() => {
  if (!hasActions && props.duration > 0) {
    setTimeout(close, props.duration);
  }
});
</script>

<style scoped>
.ms-toast {
  width: 560px;
  max-width: calc(100vw - 48px);
  min-width: 320px;
  padding: 12px 16px;
  border-radius: 4px;
  background-color: white;
  box-shadow: 0 4px 12px rgba(0,0,0,0.15);
  border-left: 4px solid transparent;
}

.success { border-left-color: var(--color-success); }
.error { border-left-color: var(--color-error); }
.warning { border-left-color: var(--color-warning); }

.toast-icon {
  width: 24px;
  height: 24px;
  background-image: url('../../assets/img/Icon.c487640.svg');
}

.toast-main {
  width: 100%;
}

.toast-actions {
  margin-top: 10px;
  justify-content: flex-start; /* 2 nÃºt á»Ÿ gÃ³c trÃ¡i */
}

.toast-text {
  font-size: 14px;
  color: var(--text-primary);
}

.toast-close {
  margin-left: auto;
  cursor: pointer;
  font-size: 20px;
  color: #999;
}

.toast-enter-active, .toast-leave-active {
  transition: all 0.3s ease;
}
.toast-enter-from {
  transform: translateY(-12px);
  opacity: 0;
}
.toast-leave-to {
  opacity: 0;
}

/* Icon offsets */
.icon-success { background-position: -992px -456px; }
.icon-error-toast { background-position: -992px -504px; }
</style>

