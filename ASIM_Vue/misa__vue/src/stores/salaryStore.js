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

const getField = (item, pascalName) => {
  const camelName = pascalName.charAt(0).toLowerCase() + pascalName.slice(1);
  return item?.[camelName] ?? item?.[pascalName];
};

const GRID_CONFIG_TABLE_NAME = 'salary_composition_list';

const normalizeColumnKey = (value) => String(value || '').replace(/_/g, '').toLowerCase();

const toFixedStatus = (column) => {
  if (!column.fixed) return 0;
  return column.fixedPosition === 'right' ? 2 : 1;
};

const fromFixedStatus = (status) => {
  const numericStatus = Number(status) || 0;
  if (numericStatus === 2) return { fixed: true, fixedPosition: 'right' };
  if (numericStatus === 1) return { fixed: true, fixedPosition: 'left' };
  return { fixed: false, fixedPosition: undefined };
};

const unwrapServiceResult = (response) => {
  const payload = response?.data ?? response;
  if (payload?.isSuccess === false || payload?.IsSuccess === false) {
    const error = new Error(payload.userMsg || payload.UserMsg || payload.devMsg || payload.DevMsg || 'API request failed');
    error.serviceResult = payload;
    throw error;
  }
  return payload?.data ?? payload?.Data ?? payload;
};

const mapSalaryComposition = (item) => {
  const rawFormula = (getField(item, 'SalaryCompositionValueFormula') ?? '').toString().trim();
  const formulaExpression = rawFormula.startsWith('=') ? rawFormula.slice(1) : rawFormula;
  const activeStatus = Number(getField(item, 'SalaryCompositionActiveStatus'));
  const systemStatus = Number(getField(item, 'SalaryCompositionIsSystemStatus'));
  const componentType = Number(getField(item, 'SalaryCompositionComponentType'));
  const natureType = Number(getField(item, 'SalaryCompositionNatureType'));
  const dataType = Number(getField(item, 'SalaryCompositionDataType'));
  const valueType = Number(getField(item, 'SalaryCompositionValueType'));
  const payslipStatus = Number(getField(item, 'SalaryCompositionPayslipStatus'));
  const isSystem = systemStatus === 1;

  return {
    ...item,
    SalaryCompositionId: getField(item, 'SalaryCompositionId'),
    SalaryCompositionCode: getField(item, 'SalaryCompositionCode') || '',
    SalaryCompositionName: getField(item, 'SalaryCompositionName') || '',
    OrganizationId: getField(item, 'OrganizationId') || null,
    AppliedUnit: getField(item, 'OrganizationName') || '',
    SalaryCompositionType: MAP_COMPONENT_TYPE[componentType] || 'Khác',
    Nature: MAP_NATURE[natureType] || 'Khác',
    KieuGiaTri: MAP_DATA_TYPE[dataType] || 'Chuỗi',
    GiaTri: formulaExpression || '-',
    NguonTao: isSystem ? 'Mặc định' : 'Tự thêm',
    StatusCode: activeStatus,
    Status: activeStatus === 1 ? 'Đang theo dõi' : 'Ngừng theo dõi',
    Quota: getField(item, 'SalaryCompositionQuotaFormula') || '',
    AllowOverQuota: Number(getField(item, 'SalaryCompositionAllowExceedStatus')) === 1,
    ValueType: MAP_DATA_TYPE[dataType] || 'Tiền tệ',
    ValueSource: valueType === 1 ? 'AutoSum' : (valueType === 3 ? 'Constant' : 'Formula'),
    Value: getField(item, 'SalaryCompositionValueFormula') || '',
    Description: getField(item, 'SalaryCompositionDescription') || '',
    DisplayOnPayslip: payslipStatus === 1 ? 'Có' : (payslipStatus === 2 ? 'Khác 0' : 'Không'),
    Source: isSystem ? 'Hệ thống' : 'Tự thêm',
    TaxStatus: MAP_TAX_STATUS[natureType] || '',
  };
};

const mapSystemComponent = (item) => {
  const componentType = Number(getField(item, 'SalarySystemComponentType'));
  const natureType = Number(getField(item, 'SalarySystemNatureType'));
  const dataType = Number(getField(item, 'SalarySystemDataType'));

  return {
    ...item,
    salarySystemId: getField(item, 'SalarySystemId'),
    salarySystemNatureType: natureType,
    salarySystemDataType: dataType,
    salarySystemComponentType: componentType,
    SalaryCompositionCode: getField(item, 'SalarySystemCode') || '',
    SalaryCompositionName: getField(item, 'SalarySystemName') || '',
    SalaryCompositionType: MAP_COMPONENT_TYPE[componentType] || 'Khác',
    Nature: MAP_NATURE[natureType] || 'Khác',
    ValueType: MAP_DATA_TYPE[dataType] || 'Chuỗi',
    Value: getField(item, 'SalarySystemValueFormula') || '',
    Description: getField(item, 'SalarySystemDescription') || '',
  };
};

export const useSalaryStore = defineStore('salary', {
  state: () => ({
    salaryCompositions: [],
    loading: false,
    lastError: null,
    columns: [
      { dataField: 'SalaryCompositionCode', caption: 'Mã thành phần', width: 250, minWidth: 100, maxWidth: 300, visible: true, fixed: true, fixedPosition: 'left', sortOrder: 1 },
      { dataField: 'SalaryCompositionName', caption: 'Tên thành phần', width: 350, minWidth: 150, maxWidth: 450, visible: true, fixed: true, fixedPosition: 'left', sortOrder: 2 },
      { dataField: 'AppliedUnit', caption: 'Đơn vị áp dụng', width: 200, minWidth: 120, maxWidth: 300, visible: true },
      { dataField: 'SalaryCompositionType', caption: 'Loại thành phần', width: 250, minWidth: 120, maxWidth: 350, visible: true },
      { dataField: 'Nature', caption: 'Tính chất', width: 200, minWidth: 100, maxWidth: 300, visible: true, cellTemplate: 'natureTemplate' },
      { dataField: 'KieuGiaTri', caption: 'Kiểu giá trị', width: 150, minWidth: 100, maxWidth: 200, visible: true },
      { dataField: 'GiaTri', caption: 'Giá trị', minWidth: 260, width: 360, visible: true, cellTemplate: 'valueTemplate' },
      { dataField: 'NguonTao', caption: 'Nguồn tạo', width: 140, minWidth: 120, maxWidth: 200, visible: true },
      { dataField: 'Status', caption: 'Trạng thái', width: 160, minWidth: 130, maxWidth: 200, visible: true, cellTemplate: 'statusTemplate' },
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

      const items = resultData.items ?? resultData.Items ?? resultData.data ?? resultData.Data ?? resultData.records ?? resultData.Records ?? [];
      const total = resultData.totalRecords ?? resultData.TotalRecords ?? resultData.totalCount ?? resultData.TotalCount ?? resultData.total ?? resultData.Total ?? resultData.count ?? resultData.Count ?? (Array.isArray(items) ? items.length : 0);
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

    reorderColumns(newColumns) {
      this.columns = newColumns;
    },

    applyGridConfigs(configs) {
      if (!Array.isArray(configs) || configs.length === 0) return;

      const configByColumn = new Map(
        configs.map((config) => [
          normalizeColumnKey(getField(config, 'GridConfigColumnName')),
          config,
        ])
      );

      this.columns = this.columns
        .map((column, index) => {
          const config = configByColumn.get(normalizeColumnKey(column.dataField));
          if (!config) return { ...column, sortOrder: column.sortOrder ?? index + 1 };

          const fixedState = fromFixedStatus(getField(config, 'GridConfigFixedStatus'));
          return {
            ...column,
            width: Number(getField(config, 'GridConfigWidthSize')) || column.width,
            visible: Number(getField(config, 'GridConfigVisibleStatus')) !== 0,
            ...fixedState,
            sortOrder: Number(getField(config, 'GridConfigSortOrder')) || index + 1,
          };
        })
        .sort((a, b) => (a.sortOrder ?? 9999) - (b.sortOrder ?? 9999));
    },

    applyColumnRuntimeState(states) {
      if (!Array.isArray(states) || states.length === 0) return;

      const stateByField = new Map(states.map((state) => [state.dataField, state]));

      this.columns = this.columns
        .map((column, index) => {
          const state = stateByField.get(column.dataField);
          if (!state) return { ...column, sortOrder: column.sortOrder ?? index + 1 };

          return {
            ...column,
            width: Number(state.width) || column.width,
            visible: state.visible !== false,
            fixed: !!state.fixed,
            fixedPosition: state.fixed ? (state.fixedPosition || 'left') : undefined,
            sortOrder: Number.isFinite(state.visibleIndex) ? state.visibleIndex + 1 : column.sortOrder ?? index + 1,
          };
        })
        .sort((a, b) => (a.sortOrder ?? 9999) - (b.sortOrder ?? 9999));
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

    async saveGridConfigs() {
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
      const local = this.salaryCompositions.find((i) => i.SalaryCompositionId === id);
      if (local) {
        this.currentItem = { ...local };
        return;
      }

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

    async saveSalaryComposition(item, isEdit = false) {
      this.loading = true;
      this.lastError = null;
      try {
        let natureType = 5;
        if (item.Nature === 'Thu nhập') {
          natureType = item.TaxStatus === 'Miễn thuế toàn phần' ? 2 : 1;
        } else if (item.Nature === 'Khấu trừ') {
          natureType = 3;
        }

        const payload = {
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
          salaryCompositionValueFormula: item.Value || '',
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
        await this.fetchSalaryCompositions();
        return true;
      } catch (error) {
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

      const normalizedCode = status === 0 || status === 1 ? Number(status) : (String(status).startsWith('Ng') ? 0 : 1);

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
