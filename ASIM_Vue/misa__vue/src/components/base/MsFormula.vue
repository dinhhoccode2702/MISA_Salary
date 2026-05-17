<template>
  <div class="formula-wrapper" :class="{ 'has-error': displayError, 'is-disabled': disabled }">
    <label v-if="label" class="ms-label">{{ label }}</label>

    <div
      class="formula-editor-shell"
      :class="{ focused: isFocused || showSuggestions }"
    >
      <prism-editor
        ref="editorRef"
        class="ms-formula-editor"
        :model-value="formulaValue"
        :highlight="highlighter"
        :line-numbers="false"
        :readonly="disabled"
        :placeholder="placeholder"
        @update:model-value="handleInput"
        @keydown="handleKeydown"
        @focus="isFocused = true"
        @blur="handleBlur"
      />
    </div>

    <div
      v-if="showSuggestions && !disabled"
      class="formula-suggestion-panel"
    >
      <div class="formula-tabs">
        <div class="formula-tab active">Công thức</div>
        <div class="formula-tab">Tham số</div>
      </div>

      <div class="formula-list">
        <div
          v-for="(param, index) in filteredSuggestions"
          :key="param"
          class="formula-item"
          :class="{ active: index === selectedSuggestionIndex }"
          @mousedown.prevent="insertSuggestion(param)"
        >
          <div class="formula-icon">fx</div>
          <div class="formula-content">
            <div class="formula-name">
              {{ param }}
              <span class="formula-signature">{{ getFormulaSignature(param) }}</span>
            </div>
          </div>
        </div>
      </div>
    </div>

    <span v-if="displayError" class="error-msg">{{ displayError }}</span>
    <span
      v-else-if="validationMessage"
      class="formula-status"
      :class="`is-${validationStatus}`"
    >
      {{ validationMessage }}
    </span>
  </div>
</template>

<script setup>
import { computed, nextTick, ref, watch } from 'vue';
import { PrismEditor } from 'vue-prism-editor';
import 'vue-prism-editor/dist/prismeditor.min.css';
import Prism from 'prismjs';

const props = defineProps({
  modelValue: String,
  label: String,
  placeholder: { type: String, default: 'Tự động gợi ý công thức và tham số khi gõ' },
  error: String,
  disabled: Boolean,
  rows: { type: Number, default: 4 },
});

const emit = defineEmits(['update:modelValue', 'validated']);

const suggestedParams = [
  'LUONG_CO_BAN',
  'TONG_CONG',
  'NGAY_CONG_THUC_TE',
  'HE_SO_LUONG',
  'BHXH',
  'BHYT',
  'BHTN',
  'PC_AN_TRUA',
  'PC_DIEN_THOAI',
  'TONG_CONG_LUONG',
  'TONG_KHAU_TRU',
  'THUC_LINH',
  'ROUND',
  'IF',
  'IFS',
  'SUM',
  'MIN',
  'MAX',
  'AND',
  'OR',
  'NOT',
  'COUNT',
  'AVERAGE',
];

const formulaDescriptions = {
  SUM: '(X1, X2, ...)',
  IF: '(Logical_test, [value_if_true], [value_if_false])',
  IFS: '(logical_test1, value_if_true1, ...)',
  ROUND: '(number, num_digits)',
  MIN: '(X1, X2, ...)',
  MAX: '(X1, X2, ...)',
  AND: '(logical1, logical2, ...)',
  OR: '(logical1, logical2, ...)',
  NOT: '(logical)',
  COUNT: '(X1, X2, ...)',
  AVERAGE: '(X1, X2, ...)',
};

Prism.languages.formula = {
  function: {
    pattern: /\b(ROUND|IF|IFS|SUM|MIN|MAX|AND|OR|NOT|COUNT|AVERAGE)\b/i,
    alias: 'important',
  },
  parameter: {
    pattern: /\b(LUONG_CO_BAN|TONG_CONG|NGAY_CONG_THUC_TE|HE_SO_LUONG|BHXH|BHYT|BHTN|PC_AN_TRUA|PC_DIEN_THOAI|TONG_CONG_LUONG|TONG_KHAU_TRU|THUC_LINH)\b/,
    alias: 'variable',
  },
  operator: /[+\-*/=<>&|]/,
  punctuation: /[(),]/,
  number: /\b\d+(\.\d+)?\b/,
};

const editorRef = ref(null);
const showSuggestions = ref(false);
const filterText = ref('');
const selectedSuggestionIndex = ref(0);
const isFocused = ref(false);
const localError = ref('');
const validationMessage = ref('');
const validationStatus = ref('');

const formulaValue = computed(() => props.modelValue || '');
const displayError = computed(() => props.error || localError.value);
const editorMinHeight = computed(() => `${(Number(props.rows) || 4) * 22 + 22}px`);

const filteredSuggestions = computed(() => {
  if (!filterText.value) return suggestedParams;
  const keyword = filterText.value.toUpperCase();
  return suggestedParams.filter((param) => param.includes(keyword));
});

watch(
  () => props.error,
  (error) => {
    if (error) {
      validationMessage.value = '';
      validationStatus.value = '';
    }
  }
);

const getFormulaSignature = (name) => formulaDescriptions[String(name || '').toUpperCase()] || '';

const getTextarea = () => editorRef.value?.$el?.querySelector?.('textarea');

const highlighter = (code) => Prism.highlight(code || '', Prism.languages.formula, 'formula');

const openSuggestionsFromValue = (value) => {
  const textarea = getTextarea();
  if (!textarea) return;

  const cursorPos = textarea.selectionStart || 0;
  const textBeforeCursor = value.substring(0, cursorPos);
  const match = textBeforeCursor.match(/[=+\-*/,()\s]?([A-Za-z_][A-Za-z0-9_]*)$/);

  if (match?.[1]) {
    filterText.value = match[1];
    showSuggestions.value = filteredSuggestions.value.length > 0;
    selectedSuggestionIndex.value = 0;
    return;
  }

  showSuggestions.value = false;
  filterText.value = '';
};

const handleInput = (value) => {
  localError.value = '';
  validationMessage.value = '';
  validationStatus.value = '';
  emit('update:modelValue', value);
  nextTick(() => openSuggestionsFromValue(value || ''));
};

const insertSuggestion = (param) => {
  const textarea = getTextarea();
  if (!textarea) return;

  const cursorPos = textarea.selectionStart || 0;
  const value = props.modelValue || '';
  const textBeforeCursor = value.substring(0, cursorPos);
  const match = textBeforeCursor.match(/[=+\-*/,()\s]?([A-Za-z_][A-Za-z0-9_]*)$/);

  if (match?.[1]) {
    const startPos = cursorPos - match[1].length;
    const newValue = value.substring(0, startPos) + param + value.substring(cursorPos);
    emit('update:modelValue', newValue);

    nextTick(() => {
      const newCursorPos = startPos + param.length;
      textarea.setSelectionRange(newCursorPos, newCursorPos);
      textarea.focus();
    });
  }

  showSuggestions.value = false;
  filterText.value = '';
};

const handleKeydown = (event) => {
  if (!showSuggestions.value || filteredSuggestions.value.length === 0) return;

  if (event.key === 'ArrowDown') {
    event.preventDefault();
    selectedSuggestionIndex.value = Math.min(
      selectedSuggestionIndex.value + 1,
      filteredSuggestions.value.length - 1
    );
  } else if (event.key === 'ArrowUp') {
    event.preventDefault();
    selectedSuggestionIndex.value = Math.max(selectedSuggestionIndex.value - 1, 0);
  } else if (event.key === 'Enter') {
    event.preventDefault();
    insertSuggestion(filteredSuggestions.value[selectedSuggestionIndex.value]);
  } else if (event.key === 'Escape') {
    showSuggestions.value = false;
  }
};

const handleBlur = () => {
  isFocused.value = false;
  setTimeout(() => {
    showSuggestions.value = false;
  }, 200);
};

const focus = () => {
  getTextarea()?.focus();
};

const removeQuotedText = (formula) => formula.replace(/"[^"\n]*"|'[^'\n]*'/g, '');

const validatePairs = (formula, openChar, closeChar, errorMessage) => {
  let count = 0;
  for (const char of formula) {
    if (char === openChar) count += 1;
    if (char === closeChar) count -= 1;
    if (count < 0) return errorMessage;
  }
  return count === 0 ? '' : errorMessage;
};

const validateFormula = (rawFormula) => {
  const formula = rawFormula.trim();
  if (!formula) return 'Vui lòng nhập công thức cần kiểm tra.';

  const normalizedFormula = formula.startsWith('=') ? formula.slice(1).trim() : formula;
  if (!normalizedFormula) return 'Công thức phải có biểu thức sau dấu bằng.';

  const formulaWithoutText = removeQuotedText(normalizedFormula);
  const invalidChar = formulaWithoutText.match(/[^A-Za-z0-9_\s()[\]+\-*\/^=,.;<>]/);
  if (invalidChar) return `Công thức chứa ký tự không hợp lệ: ${invalidChar[0]}.`;

  const bracketError = validatePairs(formulaWithoutText, '[', ']', 'Cặp ngoặc vuông của mã thành phần lương chưa hợp lệ.');
  if (bracketError) return bracketError;

  const parenthesisError = validatePairs(formulaWithoutText, '(', ')', 'Cặp ngoặc tròn trong công thức chưa hợp lệ.');
  if (parenthesisError) return parenthesisError;

  if (/[\+\-*\/^,;.]$/.test(formulaWithoutText.trim())) {
    return 'Công thức không được kết thúc bằng toán tử hoặc dấu phân tách.';
  }

  if (/^[*\/^,;.]/.test(formulaWithoutText.trim())) {
    return 'Công thức không được bắt đầu bằng toán tử hoặc dấu phân tách.';
  }

  if (/[\+\-*\/^]{2,}/.test(formulaWithoutText.replace(/\s+/g, ''))) {
    return 'Công thức có toán tử liên tiếp chưa hợp lệ.';
  }

  return '';
};

const checkLogic = () => {
  const errorMessage = validateFormula(formulaValue.value);
  localError.value = errorMessage;
  validationStatus.value = errorMessage ? 'error' : 'success';
  validationMessage.value = errorMessage ? '' : 'Công thức hợp lệ.';
  emit('validated', { valid: !errorMessage, message: errorMessage });
};

defineExpose({ focus, checkLogic });
</script>

<style scoped>
.formula-wrapper {
  width: 100%;
  position: relative;
}

.ms-label {
  display: block;
  font-size: 13px;
  font-weight: 700;
  margin-bottom: 4px;
  color: #111;
}

.formula-editor-shell {
  display: flex;
  align-items: flex-start;
  border: 1px solid var(--border-color);
  border-radius: 4px;
  background: #fff;
  min-height: v-bind(editorMinHeight);
  overflow: hidden;
  transition: border-color 0.2s ease, box-shadow 0.2s ease;
}

.formula-editor-shell.focused {
  border-color: var(--color-primary);
  box-shadow: 0 0 0 1px rgba(44, 160, 28, 0.13);
}

.has-error .formula-editor-shell {
  border-color: var(--color-error);
}

.is-disabled .formula-editor-shell {
  background-color: #f3f4f6;
}

.ms-formula-editor {
  flex: 1;
  position: relative;
  background: transparent;
  color: #222;
  min-height: inherit;
  font-family: Consolas, Monaco, monospace;
  font-size: 13px;
  line-height: 22px;
}

:deep(.prism-editor__textarea) {
  position: absolute !important;
  inset: 0 !important;
  padding: 8px 10px !important;
  color: transparent !important;
  caret-color: var(--text-primary) !important;
  background: transparent !important;
  resize: none !important;
  outline: none !important;
  border: none !important;
  overflow: hidden !important;
  z-index: 2;
  font-family: Consolas, Monaco, monospace !important;
  font-size: 13px !important;
  line-height: 22px !important;
}

:deep(.prism-editor__textarea::placeholder) {
  font-size: 13px;
  color: #c4c8cf;
  opacity: 1;
}

:deep(.prism-editor__container) {
  min-height: inherit;
  padding: 8px 10px;
}

:deep(.prism-editor__editor),
:deep(.prism-editor__code) {
  min-height: inherit;
  color: #222 !important;
  pointer-events: none;
  font-family: Consolas, Monaco, monospace !important;
  font-size: 13px !important;
  line-height: 22px !important;
}

:deep(pre),
:deep(code) {
  margin: 0 !important;
  background: transparent !important;
  text-shadow: none !important;
  font-family: Consolas, Monaco, monospace !important;
}

:deep(.token) {
  background: none !important;
  text-shadow: none !important;
}

.formula-suggestion-panel {
  margin-top: 12px;
  background: #f7f7f7;
  border-radius: 8px;
  border: 1px solid #e4e4e4;
  overflow: hidden;
  box-shadow: 0 8px 20px rgba(0, 0, 0, 0.08);
}

.formula-tabs {
  display: flex;
  gap: 28px;
  padding: 12px 24px 0;
  background: #f7f7f7;
}

.formula-tab {
  position: relative;
  padding-bottom: 12px;
  font-size: 13px;
  color: var(--text-secondary);
  cursor: pointer;
}

.formula-tab.active {
  color: var(--color-primary);
  font-weight: 600;
}

.formula-tab.active::after {
  content: '';
  position: absolute;
  left: 0;
  bottom: 0;
  width: 100%;
  height: 3px;
  border-radius: 999px;
  background: var(--color-primary);
}

.formula-list {
  max-height: 260px;
  overflow-y: auto;
  padding: 10px 0;
}

.formula-item {
  display: flex;
  align-items: flex-start;
  padding: 10px 24px;
  gap: 10px;
  cursor: pointer;
  transition: background 0.15s ease;
}

.formula-item:hover,
.formula-item.active {
  background: #ececec;
}

.formula-icon {
  font-size: 20px;
  color: var(--text-secondary);
  font-family: serif;
  line-height: 1;
  margin-top: 2px;
}

.formula-content {
  flex: 1;
}

.formula-name {
  font-size: 13px;
  color: var(--text-primary);
  font-weight: 700;
}

.formula-signature {
  font-weight: 400;
  color: var(--text-secondary);
  margin-left: 6px;
  font-size: 13px;
}

.error-msg,
.formula-status {
  font-size: 12px;
  margin-top: 4px;
}

.error-msg,
.formula-status.is-error {
  color: var(--color-error);
}

.formula-status.is-success {
  color: var(--color-primary);
}

:deep(.token.function) {
  color: #1565c0;
  font-weight: 700;
}

:deep(.token.parameter) {
  color: #2e7d32;
}

:deep(.token.operator) {
  color: #c62828;
}

:deep(.token.number) {
  color: #ef6c00;
}

:deep(.token.punctuation) {
  color: var(--text-secondary);
}
</style>
