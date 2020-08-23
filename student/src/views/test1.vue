<template>
	<div style="width: 100%;">
		<router-link to="/test2">选项2</router-link>
		<el-col :span="24" class="toolbar" style="padding-bottom: 0px;">
		      <el-form :inline="true">
		        <el-form-item>
		          <el-input placeholder="关键字" v-model="selectName"></el-input>
		        </el-form-item>
		        <el-form-item>
		          <el-button type="primary" @click="selectDialog()">查询</el-button>
		        </el-form-item>
		        <el-form-item>
		         <router-link to="/AddOrUpdate"> <el-button type="primary">新增</el-button></router-link>
		        </el-form-item>
		      </el-form>
		    </el-col>
	    <el-main>
	      <el-table :data="tableData">
	        <el-table-column prop="uid" label="编号" width="140">
	        </el-table-column>
	        <el-table-column prop="userName" label="用户名" width="120">
	        </el-table-column>
	        <el-table-column prop="userPwd" label="密码">
	        </el-table-column>
			<el-table-column prop="address" label="地址">
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
		selectName:''
      }
    },
	methods: {
	      getUsers() {      //这个方法输给users填充数据的，但是没法直接初始化时就调用
	        this.axios.get('/Users').then((response) => {
				console.log(response.data);
	          this.tableData = response.data;
	        })
	      },
		  removeUserById(data){
			  console.log(data);
			  this.axios.delete('/Users/'+data).then((response) => {
			  	console.log(response.data);
				 this.getUsers();
			  })
		  },
		  selectDialog(){
			  if(Object.keys(this.selectName).length==0)
			  { 
				  this.getUsers();
			  }else{ 
				  this.axios.get('/Users/'+this.selectName).then((response) => {
			  	 console.log(response.data);
				  this.tableData=[];
				   this.tableData.push(response.data); 
			  });
			  }
		  },
		  showEditDialog(data){
			  this.$router.push({ path: '/AddOrUpdate/'+data });
		  }
	},
	mounted () { //这个属性就可以，在里面声明初始化时要调用的方法即可
	      // we can implement any method here like
	      this.getUsers()
	    }
  };
</script>
