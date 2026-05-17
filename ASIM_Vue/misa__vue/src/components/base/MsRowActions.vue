<template>

  <div class="row-actions flex align-center">
    <button
      v-for="action in actions"
      :key="action.key"
      class="action-btn"
      :class="[
        `action-btn--${action.variant || 'default'}`,
        { 'action-btn--no-hover': action.noHoverBg }
      ]"
      :aria-label="action.label"
      :data-tooltip="action.label"
      @click.stop="$emit('action', action.key)"
    >
      <!-- Icon dùng webkit-mask, màu thay đổi theo variant hoặc color prop -->
      <span
        class="ms-icon-base action-icon"
        :class="`ms-icon--${action.icon}`"
        :style="action.color ? { backgroundColor: action.color } : {}"
      ></span>
    </button>
  </div>
</template>

<script setup>
// ── Props ──────────────────────────────────────────────────
defineProps({
  /**
   * Danh sách các nút hành động.
   * Mỗi phần tử: { key, icon, label, variant, color, noHoverBg }
   * - key: định danh, dùng để emit event
   * - icon: tên icon (khớp với ms-icon--{icon} trong icons.css)
   * - label: tooltip text
   * - variant: 'default' (xanh/xám) | 'danger' (đỏ)
   * - color: màu icon thủ công (ví dụ: 'green', '#00ff00')
   * - noHoverBg: true nếu không muốn đổi màu background của button khi hover
   */
  actions: {
    type: Array,
    default: () => [],
  },
});

// ── Emits ──────────────────────────────────────────────────
defineEmits(['action']);
</script>

<style scoped>
/* ── Container: mặc định ẩn, hiện khi :deep(.dx-row:hover) ── */
.row-actions {
  display: flex;
  gap: 2px;
  padding: 4px 8px;
  /* Gradient mờ để blend vào nền row */
  background: linear-gradient(to right, transparent, #f2f9f2 20%, #f2f9f2);
  border-radius: 0 0 0 4px;
  z-index: 1;
}

/* ── Nút action ── */
.action-btn {
  width: 28px;
  height: 28px;
  border-radius: 4px;
  border: none;
  background: none;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: background-color 0.15s;
}

.action-btn:hover {
  background-color: rgba(0, 0, 0, 0.06);
}

/* Disable hover background */
.action-btn--no-hover:hover {
  background-color: transparent !important;
}

/* ── Icon màu theo variant ── */

/* Default: màu xanh/xám MISA */
.action-btn--default .action-icon {
  background-color: #5a5a6e;
}
.action-btn--default:hover .action-icon {
  background-color: var(--color-primary);
}

/* Force keep custom color on hover if specified in style */
/* .action-btn:hover .action-icon[style*="background-color"] {
  background-color: inherit !important;
} */
/* Note: inline style usually wins, but just in case of hover rules */

/* Danger: màu đỏ xóa */
.action-btn--danger .action-icon {
  background-color: #bdbdbd; /* Mặc định xám */
}
.action-btn--danger:hover .action-icon {
  background-color: #eb5757; /* Đỏ khi hover */
}


/* Warning: màu vàng (cho nút Ngừng theo dõi) */
.action-btn--warning .action-icon {
  background-color: #f2c94c; /* Vàng mặc định */
}
.action-btn--warning:hover .action-icon {
  background-color: #f2994a; /* Cam-vàng khi hover */
}

.action-btn--success .action-icon {
  background-color: #6b7280;
}

.action-btn--success:hover .action-icon {
  background-color: var(--color-primary);
}

/* Kích thước icon trong action */
.action-icon {
  width: 18px;
  height: 18px;
}
</style>
