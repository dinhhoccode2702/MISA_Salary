import { ref } from 'vue';

const toasts = ref([]);
const confirms = ref([]);

export function useToast() {
  const createId = () => `${Date.now()}-${Math.random().toString(16).slice(2)}`;

  const show = (messageOrOptions, type = 'success', duration = 3000) => {
    const isOptionsObject = messageOrOptions && typeof messageOrOptions === 'object';
    const options = isOptionsObject
      ? messageOrOptions
      : { message: messageOrOptions, type, duration };

    const id = options.id || createId();
    const toast = {
      id,
      message: options.message || '',
      type: options.type || 'success',
      duration: Number.isFinite(options.duration) ? Number(options.duration) : 3000,
      dismissible: options.dismissible !== false,
      actions: Array.isArray(options.actions) ? options.actions : null,
      onClose: null,
    };

    toast.onClose = () => remove(id);
    toasts.value.push(toast);

    if (!toast.actions && toast.duration > 0) {
      setTimeout(() => {
        remove(id);
      }, toast.duration + 500);
    }
  };

  const remove = (id) => {
    const index = toasts.value.findIndex(t => t.id === id);
    if (index !== -1) {
      toasts.value.splice(index, 1);
    }
  };

  const confirm = ({
    title = 'Thông báo',
    message,
    cancelLabel = 'Hủy',
    okLabel = 'Đồng ý',
    okVariant = 'primary',
    width = '420px',
  } = {}) => {
    return new Promise((resolve) => {
      const id = createId();
      confirms.value.push({
        id,
        title,
        message: message || '',
        cancelLabel,
        okLabel,
        okVariant,
        width,
        resolve,
      });
    });
  };

  const closeConfirm = (id, result = false) => {
    const index = confirms.value.findIndex(t => t.id === id);
    if (index === -1) return;

    const [confirmItem] = confirms.value.splice(index, 1);
    confirmItem.resolve?.(result);
  };

  return {
    toasts,
    confirms,
    show,
    remove,
    confirm,
    closeConfirm,
  };
}
