import { defineStore } from 'pinia';
import salaryService from '../services/salaryService';
import salarySystemService from '../services/salarySystemService';
import gridConfigService from '../services/gridConfigService';
const DEFAULT_ITEM = {
  SalaryCompositionCode: '',
  SalaryCompositionName: '',
  OrganizationId: null,
  AppliedUnit: 'Tất cả đơn vị công tác',
  SalaryCompositionType: 'Lương',
  Nature: 'Thu nhập',
  TaxStatus: 'Chịu thuế',
  Quota: '',
  AllowOverQuota: false,
  ValueType: 'Tiền tệ',
  ValueSource: 'Formula',
  AutoSumScope: 'Trong cùng đơn vị công tác',
  Value: '',
  Description: '',
  DisplayOnPayslip: 'Có',
  Source: 'Tự thêm',
  Status: 'Đang theo dõi',
};

const MAP_COMPONENT_TYPE = {
  1: 'Lương',
  2: 'Phụ cấp',
  3: 'Giảm trừ',
  4: 'Chấm công',
  5: 'Thuế TNCN',
  6: 'Bảo hiểm - Công đoàn',
  7: 'Thông tin nhân viên',
};

const MAP_DATA_TYPE = {
  1: 'Số',
  2: 'Tiền tệ',
  3: 'Phần trăm',
  4: 'Ngày',
  5: 'Chuỗi',
};

const MAP_NATURE = {
  1: 'Thu nhập',
  2: 'Thu nhập',
  3: 'Khấu trừ',
  4: 'Khấu trừ',
  5: 'Khác',
};

const MAP_TAX_STATUS = {
  1: 'Chịu thuế',
  2: 'Miễn thuế toàn phần',
};

const REVERSE_MAP_TYPE = {
  'Lương': 1,
  'Phụ cấp': 2,
  'Giảm trừ': 3,
  'Chấm công': 4,
  'Thuế TNCN': 5,
  'Bảo hiểm - Công đoàn': 6,
  'Thông tin nhân viên': 7,
  'Khác': 7,
};

const REVERSE_MAP_DATA_TYPE = {
  'Số': 1,
  'Tiền tệ': 2,
  'Phần trăm': 3,
  'Ngày': 4,
  'Chuỗi': 5,
};

const normalizeColumnKey = (value) => String(value || '').replace(/_/g, '').toLowerCase();

const LEGACY_COLUMN_FIELD_ALIASES = {
  [normalizeColumnKey('KieuGiaTri')]: 'ValueType',
  [normalizeColumnKey('GiaTri')]: 'DisplayValue',
  [normalizeColumnKey('NguonTao')]: 'Source',
};

const resolveColumnField = (dataField) => {
  const normalizedField = normalizeColumnKey(dataField);
  return LEGACY_COLUMN_FIELD_ALIASES[normalizedField] || dataField;
};

const getColumnConfigKey = (dataField) => normalizeColumnKey(resolveColumnField(dataField));

const mapGridConfig = (config) => ({
  columnName: config?.columnName ?? '',
  widthSize: Number(config?.widthSize) || 0,
  visibleStatus: Number(config?.visibleStatus) || 0,
  fixedStatus: Number(config?.fixedStatus) || 0,
  sortOrder: Number(config?.sortOrder) || 0,
});

const GRID_CONFIG_TABLE_NAME = 'salary_composition_list';

// Đổi trạng thái ghim trong frontend sang số để lưu xuống pa_grid_config.
// 0: không ghim, 1: ghim trái, 2: ghim phải.
const toFixedStatus = (column) => {
  if (!column.fixed) return 0;
  return column.fixedPosition === 'right' ? 2 : 1;
};

// Đổi số lấy từ DB về cấu hình mà DevExtreme DataGrid hiểu được.
const fromFixedStatus = (status) => {
  const numericStatus = Number(status) || 0;
  if (numericStatus === 2) return { fixed: true, fixedPosition: 'right' };
  if (numericStatus === 1) return { fixed: true, fixedPosition: 'left' };
  return { fixed: false, fixedPosition: undefined };
};

// Màn này đang dùng kiểu ghim theo nhóm từ trái sang phải.
// Nếu cột C được ghim thì A, B, C đều fixed=true; C là pinAnchor để vẽ vạch ngăn.
const normalizePinnedPrefix = (columns) => {
  const pinAnchorIndex = columns.reduce((lastIndex, column, index) => {
    return column.fixed ? index : lastIndex;
  }, -1);

  return columns.map((column, index) => {
    const isInPinnedPrefix = pinAnchorIndex >= 0 && index <= pinAnchorIndex;
    return {
      ...column,
      fixed: isInPinnedPrefix,
      fixedPosition: isInPinnedPrefix ? 'left' : undefined,
      pinAnchor: pinAnchorIndex >= 0 && index === pinAnchorIndex,
    };
  });
};

const unwrapServiceResult = (response) => {
  const payload = response?.data ?? response;
  if (payload?.isSuccess === false) {
    const error = new Error(payload.userMsg || payload.devMsg || 'API request failed');
    error.serviceResult = payload;
    throw error;
  }
  return payload?.data ?? payload;
};

const isTimeoutError = (error) =>
  error?.code === 'ECONNABORTED' ||
  String(error?.message || '').toLowerCase().includes('timeout');

const mapSalaryComposition = (item) => {
  const rawValue = item?.salaryCompositionValueFormula ?? item?.SalaryCompositionValueFormula ?? item?.salaryCompositionValue ?? item?.SalaryCompositionValue ?? item?.valueFormula ?? item?.ValueFormula ?? item?.value ?? item?.Value ?? '';
  const rawFormula = (rawValue ?? '').toString().trim();
  const formulaExpression = rawFormula.startsWith('=') ? rawFormula.slice(1) : rawFormula;
  const activeStatus = Number(item?.salaryCompositionActiveStatus ?? item?.SalaryCompositionActiveStatus);
  const systemStatus = Number(item?.salaryCompositionIsSystemStatus ?? item?.SalaryCompositionIsSystemStatus);
  const componentType = Number(item?.salaryCompositionComponentType ?? item?.SalaryCompositionComponentType);
  const natureType = Number(item?.salaryCompositionNatureType ?? item?.SalaryCompositionNatureType);
  const dataType = Number(item?.salaryCompositionDataType ?? item?.SalaryCompositionDataType);
  const valueType = Number(item?.salaryCompositionValueType ?? item?.SalaryCompositionValueType);
  const payslipStatus = Number(item?.salaryCompositionPayslipStatus ?? item?.SalaryCompositionPayslipStatus);
  const isSystem = systemStatus === 1;

  return {
    ...item,
    SalaryCompositionId: item?.salaryCompositionId ?? item?.SalaryCompositionId,
    SalaryCompositionCode: item?.salaryCompositionCode ?? item?.SalaryCompositionCode ?? '',
    SalaryCompositionName: item?.salaryCompositionName ?? item?.SalaryCompositionName ?? '',
    SalaryCompositionIsSystemStatus: systemStatus,
    OrganizationId: item?.organizationId ?? item?.OrganizationId ?? null,
    AppliedUnit: item?.organizationName ?? item?.OrganizationName ?? '',
    SalaryCompositionType: MAP_COMPONENT_TYPE[componentType] || 'Khác',
    Nature: MAP_NATURE[natureType] || 'Khác',
    ValueType: MAP_DATA_TYPE[dataType] || 'Chuỗi',
    DisplayValue: formulaExpression || '-',
    StatusCode: activeStatus,
    Status: activeStatus === 1 ? 'Đang theo dõi' : 'Ngừng theo dõi',
    Quota: item?.salaryCompositionQuotaFormula ?? item?.SalaryCompositionQuotaFormula ?? '',
    AllowOverQuota: Number(item?.salaryCompositionAllowExceedStatus ?? item?.SalaryCompositionAllowExceedStatus) === 1,
    ValueSource: valueType === 1 ? 'AutoSum' : (valueType === 3 ? 'Constant' : 'Formula'),
    Value: rawValue ?? '',
    Description: item?.salaryCompositionDescription ?? item?.SalaryCompositionDescription ?? '',
    DisplayOnPayslip: payslipStatus === 1 ? 'Có' : (payslipStatus === 2 ? 'Khác 0' : 'Không'),
    Source: isSystem ? 'Hệ thống' : 'Tự thêm',
    TaxStatus: MAP_TAX_STATUS[natureType] || '',
  };
};

const mapSystemComponent = (item) => {
  const componentType = Number(item?.salarySystemComponentType ?? item?.SalarySystemComponentType);
  const natureType = Number(item?.salarySystemNatureType ?? item?.SalarySystemNatureType);
  const dataType = Number(item?.salarySystemDataType ?? item?.SalarySystemDataType);

  return {
    ...item,
    salarySystemId: item?.salarySystemId ?? item?.SalarySystemId,
    salarySystemNatureType: natureType,
    salarySystemDataType: dataType,
    salarySystemComponentType: componentType,
    SalaryCompositionCode: item?.salarySystemCode ?? item?.SalarySystemCode ?? '',
    SalaryCompositionName: item?.salarySystemName ?? item?.SalarySystemName ?? '',
    SalaryCompositionType: MAP_COMPONENT_TYPE[componentType] || 'Khác',
    Nature: MAP_NATURE[natureType] || 'Khác',
    ValueType: MAP_DATA_TYPE[dataType] || 'Chuỗi',
    Value: item?.salarySystemValueFormula ?? item?.SalarySystemValueFormula ?? '',
    Description: item?.salarySystemDescription ?? item?.SalarySystemDescription ?? '',
  };
};

export const useSalaryStore = defineStore('salary', {
  state: () => ({
    salaryCompositions: [],
    loading: false,
    lastError: null,
    columns: [
      { dataField: 'SalaryCompositionCode', caption: 'Mã thành phần', width: 250, minWidth: 100, maxWidth: 300, visible: true, fixed: false, sortOrder: 1, visibleIndex: 0 },
      { dataField: 'SalaryCompositionName', caption: 'Tên thành phần', width: 350, minWidth: 150, maxWidth: 450, visible: true, fixed: false, sortOrder: 2, visibleIndex: 1 },
      { dataField: 'AppliedUnit', caption: 'Đơn vị áp dụng', width: 200, minWidth: 120, maxWidth: 300, visible: true, fixed: false, sortOrder: 3, visibleIndex: 2 },
      { dataField: 'SalaryCompositionType', caption: 'Loại thành phần', width: 250, minWidth: 120, maxWidth: 350, visible: true, fixed: false, sortOrder: 4, visibleIndex: 3 },
      { dataField: 'Nature', caption: 'Tính chất', width: 200, minWidth: 100, maxWidth: 300, visible: true, fixed: false, sortOrder: 5, visibleIndex: 4, cellTemplate: 'natureTemplate' },
      { dataField: 'ValueType', caption: 'Kiểu giá trị', width: 150, minWidth: 100, maxWidth: 200, visible: true, fixed: false, sortOrder: 6, visibleIndex: 5 },
      { dataField: 'DisplayValue', caption: 'Giá trị', minWidth: 260, width: 360, visible: true, fixed: false, sortOrder: 7, visibleIndex: 6, cellTemplate: 'valueTemplate' },
      { dataField: 'Source', caption: 'Nguồn tạo', width: 140, minWidth: 120, maxWidth: 200, visible: true, fixed: false, sortOrder: 8, visibleIndex: 7 },
      { dataField: 'Status', caption: 'Trạng thái', width: 160, minWidth: 130, maxWidth: 200, visible: true, fixed: false, sortOrder: 9, visibleIndex: 8, cellTemplate: 'statusTemplate' },
    ],
    currentItem: { ...DEFAULT_ITEM },
    pagination: {
      currentPage: 1,
      pageSize: 50,
      total: 0,
    },
    selectedRows: [],
    searchText: '',
    filters: {
      unit: '',
      status: '',
    },
    systemComponents: [],
  }),

  getters: {
    selectedCount: (state) => state.selectedRows.length,
    visibleColumns: (state) => state.columns.filter((c) => c.visible),
  },

  actions: {
    _normalizePagingResponse(response) {
      const resultData = unwrapServiceResult(response);
      if (!resultData) return { items: [], total: 0 };
      if (Array.isArray(resultData)) return { items: resultData, total: resultData.length };

      const items = resultData.items ?? resultData.data ?? resultData.records ?? [];
      const total = resultData.totalRecords ?? resultData.totalCount ?? resultData.total ?? resultData.count ?? (Array.isArray(items) ? items.length : 0);
      return { items: Array.isArray(items) ? items : [], total: Number(total) || 0 };
    },

    setNewItem() {
      this.currentItem = { ...DEFAULT_ITEM };
    },

    setPagination(patch) {
      this.pagination = { ...this.pagination, ...patch };
    },

    setSelectedRows(rows) {
      this.selectedRows = rows;
    },

    setSearchText(text) {
      this.searchText = text;
      this.pagination.currentPage = 1;
    },

    setFilter(key, value) {
      this.filters[key] = value;
      this.pagination.currentPage = 1;
    },

    updateColumnVisibility(dataField, visible) {
      const col = this.columns.find((c) => c.dataField === dataField);
      if (col) col.visible = visible;
    },

    // dataField là tên field của cột vừa bấm icon ghim, ví dụ SalaryCompositionName.
    pinColumn(dataField) {
      const pinAnchorIndex = this.columns.findIndex((column) => column.dataField === dataField);
      if (pinAnchorIndex === -1) return;

      // Ghim toàn bộ các cột từ đầu danh sách đến cột được chọn.
      this.columns = this.columns.map((column, index) => {
        const isInPinnedPrefix = index <= pinAnchorIndex;
        return {
          ...column,
          fixed: isInPinnedPrefix,
          fixedPosition: isInPinnedPrefix ? 'left' : undefined,
          pinAnchor: index === pinAnchorIndex,
        };
      });
    },

    reorderColumns(newColumns) {
      this.columns = newColumns;
    },

    applyGridConfigs(configs) {
      if (!Array.isArray(configs) || configs.length === 0) return;

      // Tạo map theo tên cột để ghép cấu hình DB vào đúng column trong state.
      const mappedConfigs = configs.map(mapGridConfig);
      const configByColumn = new Map(
        mappedConfigs.map((config) => [
          getColumnConfigKey(config.columnName),
          config,
        ])
      );

      const nextColumns = this.columns
        .map((column, index) => {
          const config = configByColumn.get(normalizeColumnKey(column.dataField));
          // Nếu DB chưa có cấu hình cho cột này thì giữ cấu hình mặc định trong code.
          if (!config) {
            const sortOrder = column.sortOrder ?? index + 1;
            return { ...column, sortOrder, visibleIndex: sortOrder - 1 };
          }

          // sortOrder trong DB là thứ tự hiển thị cột, không phải sort dữ liệu asc/desc.
          const fixedState = fromFixedStatus(config.fixedStatus);
          const sortOrder = Number(config.sortOrder) || index + 1;
          return {
            ...column,
            width: Number(config.widthSize) || column.width,
            visible: Number(config.visibleStatus) !== 0,
            ...fixedState,
            sortOrder,
            visibleIndex: sortOrder - 1,
          };
        })
        .sort((a, b) => (a.sortOrder ?? 9999) - (b.sortOrder ?? 9999));

      // Chuẩn hóa lại thứ tự liên tục 1..n và tính lại nhóm cột đang ghim.
      this.columns = normalizePinnedPrefix(nextColumns).map((column, index) => ({
        ...column,
        sortOrder: index + 1,
        visibleIndex: index,
      }));
    },

    /**
     * Bước 5 trong luồng kéo thả cột.
     *
     * Ai gọi: SalaryCompositionList.vue sau khi nhận event columns-state-changed.
     * Nhận vào: states là mảng trạng thái cột do MsDataGrid đọc từ DevExtreme.
     *
     * Ví dụ sau khi kéo cột D lên vị trí 2:
     * states có thể là A visibleIndex=0, D visibleIndex=1, B visibleIndex=2, C visibleIndex=3.
     *
     * Việc làm:
     * - Tạo map dataField -> sortOrder mới dựa trên visibleIndex.
     * - Ghép state mới vào từng column đang lưu trong store.
     * - Sort lại this.columns theo sortOrder để mảng store có đúng thứ tự mới.
     * - Chuẩn hóa lại visibleIndex và pinAnchor.
     *
     * Kết quả:
     * this.columns được cập nhật đúng theo thứ tự người dùng vừa kéo trên UI.
     */
    applyColumnRuntimeState(states) {
      if (!Array.isArray(states) || states.length === 0) return;

      // states đến từ DevExtreme sau khi kéo cột/resize/ẩn hiện.
      // visibleIndex là vị trí mới trên grid, nên chuyển nó thành sortOrder để lưu DB.
      const visibleOrderByField = new Map(
        [...states]
          .sort((a, b) => (a.visibleIndex ?? 9999) - (b.visibleIndex ?? 9999))
          .map((state, index) => [state.dataField, index + 1])
      );
      const stateByField = new Map(states.map((state) => [state.dataField, state]));

      const nextColumns = this.columns
        .map((column, index) => {
          const state = stateByField.get(column.dataField);
          // Cột không xuất hiện trong state thì giữ nguyên cấu hình đang có.
          if (!state) return { ...column, sortOrder: column.sortOrder ?? index + 1, visibleIndex: column.visibleIndex ?? index };

          const sortOrder = visibleOrderByField.get(column.dataField) ?? column.sortOrder ?? index + 1;

          return {
            ...column,
            width: Number(state.width) || column.width,
            visible: state.visible !== false,
            fixed: !!state.fixed,
            fixedPosition: state.fixed ? (state.fixedPosition || 'left') : undefined,
            sortOrder,
            visibleIndex: sortOrder - 1,
          };
        })
        .sort((a, b) => (a.sortOrder ?? 9999) - (b.sortOrder ?? 9999));

      // Sau khi kéo cột, tính lại fixed/pinAnchor để nhóm ghim vẫn là một dải liền bên trái.
      this.columns = normalizePinnedPrefix(nextColumns).map((column, index) => ({
        ...column,
        sortOrder: index + 1,
        visibleIndex: index,
      }));
    },

    async fetchGridConfigs() {
      try {
        const response = await gridConfigService.getGridConfigs({ tableName: GRID_CONFIG_TABLE_NAME });
        const configs = unwrapServiceResult(response);
        this.applyGridConfigs(configs);
      } catch (error) {
        console.error('[salaryStore] fetchGridConfigs error:', error);
        throw error;
      }
    },

    /**
     * Bước 7 trong luồng kéo thả cột.
     *
     * Ai gọi: scheduleColumnConfigSave() hoặc các thao tác lưu cấu hình khác.
     * Việc làm:
     * - Chuyển this.columns thành payload backend cần.
     * - gridConfigSortOrder = index + 1 chính là thứ tự cột mới sau khi kéo.
     * - gridConfigFixedStatus lấy từ fixed/fixedPosition để lưu trạng thái ghim.
     * - Gửi PUT /grid-configs?tableName=salary_composition_list.
     *
     * Backend sẽ xóa cấu hình cũ của tableName rồi insert lại toàn bộ payload này.
     */
    async saveGridConfigs() {
      // API lưu theo kiểu replace toàn bộ cấu hình của tableName,
      // nên mỗi lần lưu phải gửi đủ tất cả các cột hiện có trong store.
      const payload = this.columns.map((column, index) => ({
        gridConfigTableName: GRID_CONFIG_TABLE_NAME,
        gridConfigColumnName: column.dataField,
        gridConfigColumnCaption: column.caption,
        gridConfigWidthSize: Number(column.width) || 150,
        gridConfigVisibleStatus: column.visible === false ? 0 : 1,
        gridConfigFixedStatus: toFixedStatus(column),
        gridConfigSortOrder: index + 1,
      }));

      await gridConfigService.saveGridConfigs(payload, { tableName: GRID_CONFIG_TABLE_NAME });
    },

    async fetchSystemComponents() {
      this.loading = true;
      this.lastError = null;
      try {
        const response = await salarySystemService.getAll();
        const data = unwrapServiceResult(response);
        const rawItems = Array.isArray(data) ? data : [];
        this.systemComponents = rawItems.map(mapSystemComponent);
      } catch (error) {
        this.lastError = error?.serviceResult ?? error?.response?.data ?? error;
        console.error('[salaryStore] fetchSystemComponents error:', this.lastError, error);
        this.systemComponents = [];
        throw error;
      } finally {
        this.loading = false;
      }
    },

    async fetchSalaryCompositions() {
      this.loading = true;
      this.lastError = null;
      try {
        const pageSize = Number(this.pagination.pageSize) || 50;
        const currentPage = Number(this.pagination.currentPage) || 1;
        const keyword = this.searchText?.trim();
        const params = { page: currentPage, pageSize };

        if (keyword) params.search = keyword;
        if (this.filters.status !== '' && this.filters.status !== undefined && this.filters.status !== null) params.status = Number(this.filters.status);
        if (this.filters.unit) params.organizationId = this.filters.unit;
        if (this.filters.type !== '' && this.filters.type !== undefined && this.filters.type !== null) params.type = Number(this.filters.type);
        if (this.filters.nature !== '' && this.filters.nature !== undefined && this.filters.nature !== null) params.nature = Number(this.filters.nature);

        const response = await salaryService.getPaging(params);
        const { items, total } = this._normalizePagingResponse(response);
        this.salaryCompositions = items.map(mapSalaryComposition);
        this.pagination.total = total;
      } catch (error) {
        const serviceResult = error?.serviceResult ?? error?.response?.data;
        this.lastError = {
          status: error?.response?.status,
          userMsg: serviceResult?.userMsg ?? serviceResult?.UserMsg,
          devMsg: serviceResult?.devMsg ?? serviceResult?.DevMsg ?? error?.message,
          errorCode: serviceResult?.errorCode ?? serviceResult?.ErrorCode,
          errors: serviceResult?.data ?? serviceResult?.Data ?? serviceResult?.errors ?? serviceResult?.Errors,
        };
        console.error('[salaryStore] fetchSalaryCompositions error:', this.lastError, error);
        this.salaryCompositions = [];
        this.pagination.total = 0;
      } finally {
        this.loading = false;
      }
    },

    async fetchSalaryCompositionById(id) {
      let local = this.salaryCompositions.find((i) => i.SalaryCompositionId === id);

  //     local = undefined; 

  // // In ra ngay trước khi vào IF để kiểm tra
  // console.log('Giá trị local NGAY TRƯỚC IF là:', local);
  //     if (local) {
  //       this.currentItem = { ...local };
  //       return;
  //     }

      this.loading = true;
      try {
        const response = await salaryService.getById(id);
        this.currentItem = mapSalaryComposition(unwrapServiceResult(response));
      } catch (error) {
        console.error('[salaryStore] fetchSalaryCompositionById error:', error);
        throw error;
      } finally {
        this.loading = false;
      }
    },

    async verifySaveAfterTimeout(payload, isEdit) {
      try {
        if (isEdit && payload.salaryCompositionId) {
          const response = await salaryService.getById(payload.salaryCompositionId);
          const data = unwrapServiceResult(response);
          return !!(data?.salaryCompositionId ?? data?.SalaryCompositionId);
        }

        const response = await salaryService.getPaging({
          page: 1,
          pageSize: 10,
          search: payload.salaryCompositionCode,
          organizationId: payload.organizationId,
        });
        const { items } = this._normalizePagingResponse(response);
        return items.some((item) => {
          const itemCode = item?.salaryCompositionCode ?? item?.SalaryCompositionCode;
          const itemOrgId = item?.organizationId ?? item?.OrganizationId;
          return String(itemCode || '') === String(payload.salaryCompositionCode || '') &&
            String(itemOrgId || '') === String(payload.organizationId || '');
        });
      } catch (verifyError) {
        console.warn('[salaryStore] could not verify save timeout:', verifyError);
        return false;
      }
    },

    async saveSalaryComposition(item, isEdit = false) {
      this.loading = true;
      this.lastError = null;
      let writeSucceeded = false;
      let payload = null;
      try {
        let natureType = 5;
        if (item.Nature === 'Thu nhập') {
          natureType = item.TaxStatus === 'Miễn thuế toàn phần' ? 2 : 1;
        } else if (item.Nature === 'Khấu trừ') {
          natureType = 3;
        }

        payload = {
          salaryCompositionId: item.SalaryCompositionId || '00000000-0000-0000-0000-000000000000',
          organizationId: item.OrganizationId || null,
          salaryCompositionCode: item.SalaryCompositionCode,
          salaryCompositionName: item.SalaryCompositionName,
          salaryCompositionComponentType: REVERSE_MAP_TYPE[item.SalaryCompositionType] || 7,
          salaryCompositionNatureType: natureType,
          salaryCompositionQuotaFormula: item.Quota || '',
          salaryCompositionAllowExceedStatus: item.AllowOverQuota ? 1 : 0,
          salaryCompositionDataType: REVERSE_MAP_DATA_TYPE[item.ValueType] || 1,
          salaryCompositionValueType: item.ValueSource === 'AutoSum' ? 1 : (item.ValueSource === 'Constant' ? 3 : 2),
          salaryCompositionValueFormula: item.Value ?? '',
          salaryCompositionDescription: item.Description || '',
          salaryCompositionPayslipStatus: item.DisplayOnPayslip === 'Có' ? 1 : (item.DisplayOnPayslip === 'Khác 0' ? 2 : 0),
          salaryCompositionIsSystemStatus: item.Source === 'Hệ thống' ? 1 : 0,
          salaryCompositionActiveStatus: item.Status === 'Đang theo dõi' ? 1 : 0,
        };

        if (isEdit) {
          await salaryService.update(payload.salaryCompositionId, payload);
        } else {
          await salaryService.create(payload);
        }
        writeSucceeded = true;

        try {
          await this.fetchSalaryCompositions();
        } catch (refreshError) {
          console.warn('[salaryStore] save succeeded but refresh failed:', refreshError);
        }

        return true;
      } catch (error) {
        if (writeSucceeded) {
          console.warn('[salaryStore] save write succeeded but post-save step failed:', error);
          return true;
        }

        if (isTimeoutError(error)) {
          const verified = payload ? await this.verifySaveAfterTimeout(payload, isEdit) : false;
          if (verified) {
            try {
              await this.fetchSalaryCompositions();
            } catch (refreshError) {
              console.warn('[salaryStore] verified timeout save but refresh failed:', refreshError);
            }
            return true;
          }
        }

        const serviceResult = error?.serviceResult ?? error?.response?.data ?? {};
        this.lastError = {
          status: error?.response?.status,
          userMsg: serviceResult?.userMsg ?? serviceResult?.UserMsg,
          devMsg: serviceResult?.devMsg ?? serviceResult?.DevMsg ?? error?.message,
          errorCode: serviceResult?.errorCode ?? serviceResult?.ErrorCode,
          errors: serviceResult?.data ?? serviceResult?.Data ?? serviceResult?.errors ?? serviceResult?.Errors,
        };
        console.error('[salaryStore] saveSalaryComposition error:', this.lastError, error);
        return false;
      } finally {
        this.loading = false;
      }
    },

    async deleteSalaryComposition(id) {
      this.loading = true;
      try {
        await salaryService.delete(id);
        await this.fetchSalaryCompositions();
        return true;
      } catch (error) {
        console.error('[salaryStore] deleteSalaryComposition error:', error);
        throw error;
      } finally {
        this.loading = false;
      }
    },

    async deleteSalaryCompositions(ids) {
      const idList = Array.isArray(ids) ? ids.filter(Boolean) : [];
      if (idList.length === 0) return true;

      this.loading = true;
      try {
        await salaryService.deleteBatch(idList);
        await this.fetchSalaryCompositions();
        this.selectedRows = [];
        return true;
      } catch (error) {
        console.error('[salaryStore] deleteSalaryCompositions error:', error);
        await this.fetchSalaryCompositions();
        throw error;
      } finally {
        this.loading = false;
      }
    },

    async duplicateSalaryComposition(id) {
      this.loading = true;
      try {
        await salaryService.clone(id);
        await this.fetchSalaryCompositions();
        return true;
      } catch (error) {
        console.error('[salaryStore] duplicateSalaryComposition error:', error);
        throw error;
      } finally {
        this.loading = false;
      }
    },

    async updateSalaryCompositionStatus(id, status) {
      const item = this.salaryCompositions.find((i) => i.SalaryCompositionId === id);
      if (!item) return false;

      const normalizedCode = Number(status);

      this.loading = true;
      try {
        await salaryService.updateStatus(id, { status: normalizedCode });
        await this.fetchSalaryCompositions();
        return true;
      } catch (error) {
        console.error('[salaryStore] updateSalaryCompositionStatus error:', error);
        throw error;
      } finally {
        this.loading = false;
      }
    },

  },
});
