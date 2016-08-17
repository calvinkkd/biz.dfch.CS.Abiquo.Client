/**
 * Copyright 2016 d-fens GmbH
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
 
﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace biz.dfch.CS.Abiquo.Client.v1
{
    public abstract class AbiquoMediaDataTypes
    {
        private const string VERSION_TEMPLATE = "{0}+json; version {2}";

        public static string VND_ABIQUO_ACCEPTEDREQUEST = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.acceptedrequest", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_APPLICATIONS = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.applications", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_APPLICATION = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.application", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_BACKUPS = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.backups", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_BACKUP = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.backup", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_CATEGORY = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.category", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_CATEGORIES = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.categories", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_CLOUDUSAGE = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.cloudusage", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_CLOUDUSAGES = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.cloudusages", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_COSTCODE = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.costcode", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_COSTCODES = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.costcodes", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_COSTCODECURRENCY = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.costcodecurrency", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_COSTCODECURRENCIES = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.costcodecurrencies", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_CURRENCY = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.currency", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_CURRENCIES = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.currencies", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_DATACENTER = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.datacenter", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_DATACENTERS = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.datacenters", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_LIMIT = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.limit", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_LIMITS = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.limits", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_DATACENTERREPOSITORY = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.datacenterrepository", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_DATACENTERREPOSITORIES = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.datacenterrepositories", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_DATACENTERRESOURCES = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.datacenterresources", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_DATACENTERSRESOURCES = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.datacentersresources", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_DATASTORE = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.datastore", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_DATASTORES = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.datastores", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_DHCPOPTION = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.dhcpoption", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_DHCPOPTIONS = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.dhcpoptions", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_HARDDISK = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.harddisk", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_HARDDISKS = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.harddisks", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_DISKFORMATTYPES = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.diskformattypes", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_DISKFORMATTYPE = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.diskformattype", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_ENTERPRISE = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.enterprise", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_ENTERPRISES = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.enterprises", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_ENTERPRISEEXCLUSIONRULE = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.enterpriseexclusionrule", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_ENTERPRISEEXCLUSIONRULES = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.enterpriseexclusionrules", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_ENTERPRISEPROPERTIES = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.enterpriseproperties", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_ENTERPRISERESOURCES = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.enterpriseresources", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_ENTERPRISESRESOURCES = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.enterprisesresources", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_ERROR = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.error", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_ERRORS = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.errors", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_FITPOLICYRULES = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.fitpolicyrules", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_FITPOLICYRULE = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.fitpolicyrule", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_FSM = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.fsm", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_FSMS = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.fsms", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_HYPERVISORTYPE = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.hypervisortype", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_HYPERVISORTYPES = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.hypervisortypes", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_INITIATORMAPPING = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.initiatormapping", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_INITIATORMAPPINGS = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.initiatormappings", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_IP = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.ip", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_IPS = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.ips", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_JOB = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.job", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_JOBS = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.jobs", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_LICENSE = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.license", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_LICENSES = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.licenses", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_LINKS = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.links", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_LOGICSERVER = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.logicserver", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_LOGICSERVERS = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.logicservers", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_MACHINE = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.machine", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_MACHINES = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.machines", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_MACHINELOADRULE = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.machineloadrule", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_MACHINELOADRULES = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.machineloadrules", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_MACHINESTATE = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.machinestate", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_NIC = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.nic", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_NICS = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.nics", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_ORGANIZATION = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.organization", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_ORGANIZATIONS = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.organizations", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_PRICINGCOSTCODE = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.pricingcostcode", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_PRICINGCOSTCODES = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.pricingcostcodes", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_PRICINGTEMPLATE = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.pricingtemplate", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_PRICINGTEMPLATES = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.pricingtemplates", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_PRICINGTIER = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.pricingtier", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_PRICINGTIERS = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.pricingtiers", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_PRIVILEGE = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.privilege", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_PRIVILEGES = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.privileges", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_RACK = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.rack", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_RACKS = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.racks", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_REMOTESERVICE = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.remoteservice", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_REMOTESERVICES = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.remoteservices", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_ROLE = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.role", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_ROLES = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.roles", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_ROLELDAP = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.roleldap", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_ROLESLDAP = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.rolesldap", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_ROLEWITHLDAP = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.rolewithldap", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_EXTENDED_RUNLIST = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.extended-runlist", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_EXTENDED_RUNLISTS = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.extended-runlists", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_SEEOTHER = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.seeother", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_STORAGEDEVICE = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.storagedevice", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_STORAGEDEVICES = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.storagedevices", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_STORAGEPOOL = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.storagepool", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_STORAGEPOOLS = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.storagepools", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_STORAGEPOOLWITHTIER = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.storagepoolwithtier", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_STORAGEPOOLSWITHTIER = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.storagepoolswithtier", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_STORAGEPOOLWITHDEVICE = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.storagepoolwithdevice", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_STORAGEPOOLSWITHDEVICE = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.storagepoolswithdevice", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_SYSTEMPROPERTY = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.systemproperty", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_SYSTEMPROPERTIES = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.systemproperties", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_TASK = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.task", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_TASKS = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.tasks", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_TEMPLATEDEFINITION = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.templatedefinition", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_TEMPLATEDEFINITIONS = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.templatedefinitions", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_TEMPLATEDEFINITIONLIST = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.templatedefinitionlist", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_TEMPLATEDEFINITIONLISTS = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.templatedefinitionlists", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_TIER = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.tier", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_TIERS = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.tiers", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_UCSRACK = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.ucsrack", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_UCSRACKS = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.ucsracks", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_USER = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.user", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_USERS = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.users", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_USERWITHROLES = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.userwithroles", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_USERSWITHROLES = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.userswithroles", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_VIRTUALAPPLIANCE = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.virtualappliance", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_VIRTUALAPPLIANCES = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.virtualappliances", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_VIRTUALAPPLIANCESTATE = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.virtualappliancestate", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_VIRTUALAPPLIANCEPRICE = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.virtualapplianceprice", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_VIRTUALAPPRESOURCES = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.virtualappresources", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_VIRTUALAPPSRESOURCES = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.virtualappsresources", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_VIRTUALDATACENTER = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.virtualdatacenter", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_VIRTUALDATACENTERS = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.virtualdatacenters", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_VIRTUALDATACENTERRESOURCES = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.virtualdatacenterresources", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_VIRTUALDATACENTERSRESOURCES = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.virtualdatacentersresources", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_VIRTUALMACHINE = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.virtualmachine", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_VIRTUALMACHINES = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.virtualmachines", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_VIRTUALMACHINETASK = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.virtualmachinetask", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_VIRTUALMACHINEWITHNODE = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.virtualmachinewithnode", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_VIRTUALMACHINESWITHNODE = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.virtualmachineswithnode", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_VIRTUALMACHINEWITHNODEEXTENDED = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.virtualmachinewithnodeextended", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_VIRTUALMACHINESWITHNODEEXTENDED = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.virtualmachineswithnodeextended", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_VIRTUALMACHINETEMPLATE = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.virtualmachinetemplate", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_VIRTUALMACHINETEMPLATES = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.virtualmachinetemplates", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_VIRTUALMACHINESTATE = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.virtualmachinestate", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_VIRTUALMACHINEINSTANCE = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.virtualmachineinstance", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_VLAN = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.vlan", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_VLANS = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.vlans", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_VLANTAGAVAILABILITY = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.vlantagavailability", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_VIRTUALMACHINENETWORKCONFIGURATION = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.virtualmachinenetworkconfiguration", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_VIRTUALMACHINENETWORKCONFIGURATIONS = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.virtualmachinenetworkconfigurations", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_ISCSIVOLUME = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.iscsivolume", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_ISCSIVOLUMES = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.iscsivolumes", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_ISCSIVOLUMEWITHVIRTUALMACHINE = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.iscsivolumewithvirtualmachine", AbiquoClient.ABIQUO_API_VERSION);
        public static string VND_ABIQUO_ISCSIVOLUMESWITHVIRTUALMACHINE = string.Format(VERSION_TEMPLATE, "application/vnd.abiquo.iscsivolumeswithvirtualmachine", AbiquoClient.ABIQUO_API_VERSION);
    }
}
