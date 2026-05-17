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
  padding: 14px 16px;
  border-radius: 6px;
  background-color: #fff;
  box-shadow: 0 8px 24px rgba(0,0,0,0.16);
  border-left: 4px solid transparent;
  border-top: 1px solid #edf0f2;
  border-right: 1px solid #edf0f2;
  border-bottom: 1px solid #edf0f2;
}

.success { border-left-color: var(--color-success); }
.error { border-left-color: var(--color-error); }
.warning { border-left-color: var(--color-warning); }
.info { border-left-color: var(--color-primary); }

.toast-icon {
  width: 22px;
  height: 22px;
  border-radius: 50%;
  position: relative;
  flex-shrink: 0;
}

.toast-main {
  width: 100%;
  gap: 10px;
}

.toast-content {
  flex: 1;
  min-width: 0;
}

.toast-actions {
  margin-top: 10px;
  justify-content: flex-start; /* 2 nÃºt á»Ÿ gÃ³c trÃ¡i */
}

.toast-text {
  font-size: 14px;
  line-height: 20px;
  color: var(--text-primary);
}

.toast-close {
  margin-left: auto;
  cursor: pointer;
  width: 28px;
  height: 28px;
  border-radius: 4px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 20px;
  line-height: 1;
  color: #8a8f98;
}

.toast-close:hover {
  background-color: #f2f4f6;
  color: #4b5563;
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

.icon-success {
  /* width: 16px;
  height: 16px;
  -webkit-mask: url('../../assets/img/ICON_V3_1-qvutYp_o.svg') no-repeat center;
  -webkit-mask-position: -302px -62px; */
  background-color: var(--color-success);
}

.icon-success::after {
  content: '';
  position: absolute;
  left: 7px;
  top: 4px;
  width: 6px;
  height: 11px;
  border: solid #fff;
  border-width: 0 2px 2px 0;
  transform: rotate(45deg);
}

.icon-error-toast {
  background-color: var(--color-error);
}

.icon-error-toast::before,
.icon-error-toast::after {
  content: '';
  position: absolute;
  left: 6px;
  top: 10px;
  width: 10px;
  height: 2px;
  background-color: #fff;
}

.icon-error-toast::before {
  transform: rotate(45deg);
}

.icon-error-toast::after {
  transform: rotate(-45deg);
}

.icon-warning-toast,
.icon-info-toast {
  background-color: var(--color-primary);
}

.icon-warning-toast::after,
.icon-info-toast::after {
  content: '!';
  position: absolute;
  inset: 0;
  color: #fff;
  font-size: 14px;
  font-weight: 700;
  line-height: 22px;
  text-align: center;
}
</style>
