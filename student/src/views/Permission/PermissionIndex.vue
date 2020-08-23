<template>
	<div style="width: 100%;">
		<el-col :span="24" class="toolbar" style="padding-bottom: 0px;">
		      <el-form :inline="true">
		        <el-form-item>
		          <el-input placeholder="关键字" v-model="selectName"></el-input>
		        </el-form-item>
		        <el-form-item>
		          <el-button type="primary" @click="selectDialog()">查询</el-button>
		        </el-form-item>
		        <el-form-item>
		         <router-link to="/PermissionAU"> <el-button type="primary">新增</el-button></router-link>
		        </el-form-item>
		      </el-form>
		    </el-col>
	    <el-main>
	      <el-table :data="tableData">
	        <el-table-column prop="Id" label="编号" width="140">
	        </el-table-column>
			<el-table-column prop="Address" label="地址">
			</el-table-column>
		 <el-table-column label="操作">
		                    <template slot-scope="scope">
		                        <!-- 修改按钮 -->
		                        <el-button type="primary" icon="el-icon-edit" size="mini" @click="showEditDialog(scope.row.uid)"></el-button>
		                        <!-- 删除按钮 -->
		                        <el-button type="danger" icon="el-icon-delete" size="mini" @click="removeUserById(scope.row.uid)"></el-button>
		                    </template>
		                </el-table-column>
	      </el-table>
	    </el-main>
		<el-pagination
		  background
		  layout="prev, pager, next,sizes"
		   @size-change="handleSizeChange"
		   @current-change="handleCurrentChange"
		  :current-page="currentPage"
		                :page-sizes="[1,5,10]"
		                :page-size="pageSize"
		  :total="total">
		</el-pagination>
	</div>
</template>

<style>
  .el-header {
    background-color: #B3C0D1;
    color: #333;
    line-height: 60px;
  }
  
  .el-aside {
    color: #333;
  }
</style>

<script>
  export default {
    data() {
      return {
        tableData:[],
		selectName:'',
		total:0,
		currentPage: 1,//默认显示第一页
		pageSize:5,//默认每页显示10条
      }
    },
	methods: {
	      getPermission(page,size) {      //这个方法输给Permission填充数据的，但是没法直接初始化时就调用
	        this.axios.get('/Permissions?page='+page+'&pagesize='+size).then((response) => {
	         this.tableData = response.data.response.data;
			 this.total=response.data.response.dataCount;
	        })
	      },
		  removeUserById(data){
			  console.log(data);
			  this.axios.delete('/Permissions/'+data).then((response) => {
			  	console.log(response.data);
				  this.getPermission(this.currentPage,this.pageSize);
			  })
		  },
		  selectDialog(){
			  if(Object.keys(this.selectName).length==0)
			  { 
				  this.getPermission(this.currentPage,this.pageSize);
			  }else{ 
				  this.axios.get('/Permissions/'+this.selectName).then((response) => {
			  	 console.log(response.data);
				  this.tableData=[];
				   this.tableData.push(response.data); 
			  });
			  }
		  },
		  showEditDialog(data){
			  this.$router.push({ path: '/AddOrUpdate/'+data });
		  },
		  handleSizeChange(val){
					    console.log(`每页 ${val} 条`);
					    this.pageSize = val;    //动态改变
						this.getPermission(this.currentPage,this.pageSize);
		},
		handleCurrentChange(val){
					    console.log(`当前页: ${val}`);
			  this.currentPage = val;    //动态改变
		      this.getPermission(this.currentPage,this.pageSize);
		}
	},
	mounted () { //这个属性就可以，在里面声明初始化时要调用的方法即可
	      // we can implement any method here like
	      this.getPermission(1,5)
	    }
  };
</script>
