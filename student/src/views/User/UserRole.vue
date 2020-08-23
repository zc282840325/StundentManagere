 <template>

<div class="custom-tree-container">
  <div class="block">
    <p>添加角色</p>
    <el-tree
      :data="list"
      show-checkbox
      node-key="id"
      default-expand-all
      :expand-on-click-node="false"
      >
    </el-tree>
  </div>
</div>

</template>

<script>
let id = 1000;

  export default {
    data() {
	  // [{
   //      id: 1,
   //      label: '角色列表'
   //    }];
      return {
        //list: JSON.parse(JSON.stringify(list)),
		list:[]
      }
    },

    methods: {
		async getRoleList(){ 
			 this.axios.get('/Roles/GetAllRoles').then((Response)=>{
				 var result=Response.data.response;
				 var data=[];
				 result.forEach(function(i,index){
                     data.push({'id':i.Id,'label':i.RoleName});
                 });
				 this.list=[{id:5000,label:'角色列表',children:data}];
			 });
		 },
		 setChecked(){
			 
		 },
         append(data) {
        const newChild = { id: id++, label: 'testtest', children: [] };
        if (!data.children) {
          this.$set(data, 'children', []);
        }
        data.children.push(newChild);
      },

      remove(node, data) {
        const parent = node.parent;
        const children = parent.data.children || parent.data;
        const index = children.findIndex(d => d.id === data.id);
        children.splice(index, 1);
      },

      // renderContent(h, { node, data, store }) {
      //   return (
      //     <span class="custom-tree-node">
      //       <span>{node.label}</span>
      //       <span>
      //         <el-button size="mini" type="text" on-click={ () => this.append(data) }>Append</el-button>
      //         <el-button size="mini" type="text" on-click={ () => this.remove(node, data) }>Delete</el-button>
      //       </span>
      //     </span>);
      // }
    },
	mounted() {
		this.getRoleList();
	}
  };
</script>

<style>
  .custom-tree-node {
    flex: 1;
    display: flex;
    align-items: center;
    justify-content: space-between;
    font-size: 14px;
    padding-right: 8px;
  }
</style>