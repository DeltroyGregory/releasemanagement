export type ReleaseTypeName = 'Major' | 'Minor' | 'Patch' | 'Hotfix';

export interface Release {
  id: number;
  name: string;
  description: string | null;
  releaseType: ReleaseTypeName;
  status: string;
  targetDate: string | null;
  createdAt: string;
  createdByUserId: string | null;
}

export interface ReleaseDetail extends Release {
  tasks: TaskItem[];
  releaseSystems: ReleaseSystem[];
  fixVersions: FixVersion[];
  comments: Comment[];
}

export interface ReleaseCreateRequest {
  name: string;
  description?: string | null;
  releaseType: ReleaseTypeName;
  targetDate?: string | null;
}

export interface ReleaseUpdateRequest extends ReleaseCreateRequest {
  status: string;
}

export interface TaskItem {
  id: number;
  taskNumber: string;
  releaseId: number;
  title: string;
  description: string | null;
  status: string;
  assigneeUserId: string | null;
  startDate: string | null;
  endDate: string | null;
  createdAt: string;
  typeId: number | null;
  typeName: string | null;
  componentId: number | null;
  componentName: string | null;
  appNameId: number | null;
  appNameValue: string | null;
  versionId: number | null;
  versionValue: string | null;
}

export interface TaskItemCreateRequest {
  releaseId: number;
  title: string;
  description?: string | null;
  assigneeUserId?: string | null;
  startDate?: string | null;
  endDate?: string | null;
  typeId?: number | null;
  componentId?: number | null;
  appNameId?: number | null;
  versionId?: number | null;
}

export interface ReleaseSystem {
  id: number;
  releaseId: number;
  systemName: string;
  notes: string | null;
}

export interface AppVersion {
  id: number;
  systemName: string;
  versionLabel: string;
  createdAt: string;
}

export interface FixVersion {
  id: number;
  releaseId: number;
  name: string;
  startDate: string | null;
  endDate: string | null;
  jiraFixVersionId: string | null;
}

export interface Comment {
  id: number;
  releaseId: number;
  authorUserId: string;
  body: string;
  createdAt: string;
}

export interface AuthMe {
  userId: string | null;
  email: string | null;
  preferredUsername: string | null;
  roles: string[];
}

export interface User {
  id: string;
  email: string | null;
  userName: string | null;
  role: string | null;
  isActive: boolean;
  createdAt: string;
  lastLoginAt: string | null;
}

export interface UserInviteRequest {
  email: string;
  role: string;
}

export interface UserRoleUpdateRequest {
  role: string;
}

export interface PermissionKey {
  key: string;
  area: string;
  label: string;
}

export interface PermissionMatrix {
  permissions: PermissionKey[];
  roles: string[];
  grants: Record<string, string[]>;
}

export interface PermissionMatrixUpdateRequest {
  grants: Record<string, string[]>;
}

export type LookupCategory = 'TaskType' | 'Component' | 'AppName' | 'Version';

export interface LookupItem {
  id: number;
  category: LookupCategory;
  value: string;
}

export interface LookupItemCreateRequest {
  category: LookupCategory;
  value: string;
}

export interface LookupItemUpdateRequest {
  value: string;
}
